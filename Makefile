.PHONY: dev prod down logs clean

DOCKER_COMPOSE = docker-compose
DOCKER_COMPOSE_DEV = $(DOCKER_COMPOSE) -f docker-compose.yml -f docker-compose.override.yml

# Desarrollo
dev:
	$(DOCKER_COMPOSE_DEV) up

dev-build:
	$(DOCKER_COMPOSE_DEV) up --build

dev-detached:
	$(DOCKER_COMPOSE_DEV) up -d

# Producción
prod:
	$(DOCKER_COMPOSE) up -d

prod-build:
	$(DOCKER_COMPOSE) up -d --build

# Utilidades
down:
	$(DOCKER_COMPOSE) down

logs:
	$(DOCKER_COMPOSE) logs -f

clean:
	$(DOCKER_COMPOSE) down -v
	docker system prune -f

help:
	@echo "Comandos disponibles:"
	@echo "  make dev          - Inicia el ambiente de desarrollo"
	@echo "  make dev-build    - Reconstruye e inicia el ambiente de desarrollo"
	@echo "  make dev-detached - Inicia el ambiente de desarrollo en segundo plano"
	@echo "  make prod         - Inicia el ambiente de producción"
	@echo "  make prod-build   - Reconstruye e inicia el ambiente de producción"
	@echo "  make down         - Detiene y elimina los contenedores"
	@echo "  make logs         - Muestra los logs de los contenedores"
	@echo "  make clean        - Limpia todos los contenedores, imágenes y volúmenes"
	@echo "  make help         - Muestra esta ayuda" 