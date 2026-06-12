-include .env
TAG ?= $(TAG)
NAME = $(PROJECT_NAME)
REPO = bempromotora/$(NAME)

ifeq '$(findstring ;,$(PATH))' ';'
	OS := Windows
	PWD := cd
else
	OS := $(shell uname 2>/dev/null || echo Unknown)
	OS := $(patsubst CYGWIN%,Cygwin,$(OS))
	OS := $(patsubst MSYS%,MSYS,$(OS))
	OS := $(patsubst MINGW%,MSYS,$(OS))
	PWD := pwd
endif

#.PHONY: build test push shell run start stop logs clean release

default: prepare

print:
	@echo "NINABRANCH: $(NINABRANCH)"
	@echo "TEST: $(TEST)"
	@echo "SONAR: $(SONAR)"
	@echo "PROJECT_NAME:  "
	@echo "DOTNET_VER: $(DOTNET_VER)"
	@echo "BUILDCONFIG: $(BUILDCONFIG)"
	@echo "ASPNETCORE_ENVIRONMENT: $(ASPNETCORE_ENVIRONMENT)"
	@echo "PROXY: $(PROXY)"
	@echo "PROXY_BYPASS: $(PROXY_BYPASS)"

prepare:
	sed -i 's/{PROJECT_NAME}/$(PROJECT_NAME)/' sonar-project.properties
	sed -i 's/{TAG}/$(TAG)/' sonar-project.properties


build:
	docker build \
		--build-arg TAG=${TAG} \
		--build-arg PROXY=${PROXY} \
		--build-arg PROXY_BYPASS=${PROXY_BYPASS} \
		--build-arg BUILDCONFIG=${BUILDCONFIG} \
		--build-arg BUILD_NUMBER=$(BUILD_NUMBER) \
		--label BUILD_NUMBER=$(BUILD_NUMBER) \
		-t $(REPO):$(TAG) .


sonar:
	docker run --rm -t -v $(shell pwd):/src bempromotora/node:16.10-bullseye bash -c "export test=1;export http_proxy=${PROXY};export https_proxy=${PROXY};export ftp_proxy=${PROXY};export PROJECT_NAME=${PROJECT_NAME};export IMAGETAG=${TAG};/usr/bin/yarn-sonar"

applyingressinterface:
	kubectl delete -f apis/k8s-ingress.yaml -n $(NAMESPACE) || echo  "ingress nao existe"
	kubectl apply -f apis/k8s-ingress.yaml -n $(NAMESPACE)

deletedeploy:
	kubectl delete deploy $(PODNAME) -n $(NAMESPACE) --grace-period=0 --force || echo  "deploy nao existe"

applydeploy: deletedeploy
	kubectl apply -f apis/k8s-deployment.yaml -n $(NAMESPACE)
	kubectl rollout status deployment/$(PODNAME)  -n $(NAMESPACE)
	make applyingressinterface

push:
	docker push $(REPO):$(TAG)

shell:
	docker-compose exec -T web bash

test-imagem:
	make start
	sleep 30
	make logs
	make logs | grep -q "ready for start up" >/dev/null && echo "It works!" || exit 1

run:
	docker-compose up --remove-orphans

start: stop
	docker-compose up -d --remove-orphans

stop:
	docker-compose down --remove-orphans

logs:
	docker-compose logs

remove:
	docker image rm $(REPO):$(TAG) -f

release: build push
