#!/bin/bash
rm -f public/version_file.json
STR=$(grep -oP '(?<="version": ")[^"]*' package.json)
touch public/version_file.json
echo "{\"version\": \"${STR}\"}" > public/version_file.json
