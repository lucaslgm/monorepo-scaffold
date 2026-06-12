# Carrega as variáveis do arquivo .env
if [ -f .env ]; then
    export $(cat .env | grep -v '^#' | xargs)
fi

mfe_file_name=${MFE_FILE_NAME}
builded_file=$(find . -type f -name ""$mfe_file_name".main.*.js" | head -n 1)

# Verifica se o arquivo foi encontrado
if [ -n "$builded_file" ]; then
    IMPORTMAP_DEPLOYER_URL=${IMPORTMAP_DEPLOYER_URL}
    MFE_NAMESPACE=${MFE_NAMESPACE}
    builded_file=$(echo "$builded_file" | sed 's/^[\.\/]*//;s/build\///')
    MFE_PROJECT_NAME=${PROJECT_NAME}
    MFE_ENVIRONMENT_URL=${MFE_ENVIRONMENT_URL}
    url_mfe=""$MFE_ENVIRONMENT_URL"/"$MFE_PROJECT_NAME"/"$builded_file""

    # Executa o curl e armazena o código de retorno
    response=$(curl --connect-timeout 10 --max-time 30 \
        -d '{ "service":"'$MFE_NAMESPACE'","url":"'$url_mfe'" }' \
        -X PATCH "$IMPORTMAP_DEPLOYER_URL/services?env=prd&skip_url_check" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -w "%{http_code}" \
        -s -o /dev/null)

    # Verifica se o código de retorno é 2xx (sucesso)
    if [[ ! $response =~ ^2[0-9][0-9]$ ]]; then
        echo "Erro ao registrar o MFE no import-map-deployer. Código de retorno: $response"
        exit 1
    fi

    echo "MFE registrado com sucesso no import-map-deployer"

    # Executa o comando nginx em modo foreground
    nginx -g 'daemon off;'
else
    echo "Nenhum arquivo "$mfe_file_name".main.*.js encontrado na pasta build"
    exit 1
fi
