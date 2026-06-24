#!/bin/sh
# ---------------------------------------------------------------------------
# PulpePOS · instalador automático
#
#   curl -fsSL https://raw.githubusercontent.com/stevengazo/Pulperia/main/setup.sh | sh
#
# Variables opcionales (export antes de ejecutar):
#   IMAGE         imagen de la app    (def: stevengazo/pulpepos:latest)
#   INSTALL_DIR   carpeta de install  (def: $HOME/pulpepos)
#   APP_PORT      puerto público       (def: 8080)
#   BRANCH        rama del repo        (def: main)
# ---------------------------------------------------------------------------
set -eu

# ── Configuración ──────────────────────────────────────────────────────────
REPO="stevengazo/Pulperia"
BRANCH="${BRANCH:-main}"
RAW="https://raw.githubusercontent.com/${REPO}/${BRANCH}"
IMAGE="${IMAGE:-stevengazo/pulpepos:latest}"
INSTALL_DIR="${INSTALL_DIR:-$HOME/pulpepos}"
APP_PORT="${APP_PORT:-8080}"

info()  { printf '\033[1;36m›\033[0m %s\n' "$1"; }
ok()    { printf '\033[1;32m✓\033[0m %s\n' "$1"; }
warn()  { printf '\033[1;33m!\033[0m %s\n' "$1"; }
die()   { printf '\033[1;31m✗ %s\033[0m\n' "$1" >&2; exit 1; }

# Lee de la terminal real aunque el script venga por una tubería (curl | sh)
ask() { # ask <variable> <prompt> <default>
  _def="$3"
  if [ -r /dev/tty ]; then
    printf '%s [%s]: ' "$2" "${_def:-}" > /dev/tty
    read _ans < /dev/tty || _ans=""
  else
    _ans=""
  fi
  [ -z "$_ans" ] && _ans="$_def"
  eval "$1=\$_ans"
}

# ── 1. Docker ──────────────────────────────────────────────────────────────
info "Verificando Docker..."
if ! command -v docker >/dev/null 2>&1; then
  warn "Docker no está instalado. Instalando vía get.docker.com..."
  curl -fsSL https://get.docker.com | sh || die "No se pudo instalar Docker."
  ok "Docker instalado."
else
  ok "Docker presente."
fi

# Comando de compose (plugin v2 o binario v1)
if docker compose version >/dev/null 2>&1; then
  COMPOSE="docker compose"
elif command -v docker-compose >/dev/null 2>&1; then
  COMPOSE="docker-compose"
else
  die "Docker Compose no está disponible. Instala el plugin: https://docs.docker.com/compose/install/"
fi
ok "Usando: $COMPOSE"

# ── 2. Carpeta de instalación + compose ────────────────────────────────────
info "Preparando $INSTALL_DIR ..."
mkdir -p "$INSTALL_DIR"
cd "$INSTALL_DIR"

info "Descargando docker-compose.yml ..."
curl -fsSL "${RAW}/docker-compose.prod.yml" -o docker-compose.yml \
  || die "No se pudo descargar el compose desde $RAW"
ok "Compose descargado."

# ── 3. Archivo .env ────────────────────────────────────────────────────────
if [ -f .env ]; then
  ok ".env ya existe — se reutiliza (bórralo para reconfigurar)."
else
  info "Configurando credenciales..."
  ask SA_PASSWORD      "Contraseña de SQL Server"   "PulpePOS_2024!"
  ask SUPABASE_URL     "URL de Supabase"            "https://TU_PROYECTO.supabase.co"
  ask SUPABASE_ANONKEY "Anon Key de Supabase"       "TU_ANON_KEY"
  ask APP_PORT         "Puerto público de la app"   "$APP_PORT"

  cat > .env <<EOF
IMAGE=${IMAGE}
APP_PORT=${APP_PORT}
SA_PASSWORD=${SA_PASSWORD}
SUPABASE_URL=${SUPABASE_URL}
SUPABASE_ANONKEY=${SUPABASE_ANONKEY}
EOF
  chmod 600 .env
  ok ".env creado."

  case "$SUPABASE_URL" in
    *TU_PROYECTO*) warn "Recuerda editar SUPABASE_URL / SUPABASE_ANONKEY en $INSTALL_DIR/.env" ;;
  esac
fi

# ── 4. Levantar ────────────────────────────────────────────────────────────
info "Descargando imágenes..."
$COMPOSE pull

info "Levantando contenedores..."
$COMPOSE up -d

ok "PulpePOS está corriendo."
printf '\n  🌐  http://localhost:%s\n' "$APP_PORT"
printf '  📂  %s\n' "$INSTALL_DIR"
printf '\n  Comandos útiles:\n'
printf '    cd %s\n' "$INSTALL_DIR"
printf '    %s logs -f app     # ver logs\n' "$COMPOSE"
printf '    %s down            # detener\n' "$COMPOSE"
printf '    %s pull && %s up -d   # actualizar\n\n' "$COMPOSE" "$COMPOSE"
