#!/bin/bash

if [ "$#" -ne 2 ]; then
    echo "Usage: ./generate_file.sh <file-path> <file-size>"
    exit 1
fi

filePath=$1
fileSize=$2

dotnet run --configuration Release --project ./FileGenerator/FileGenerator.csproj --file-path "$filePath" --file-size-gb "$fileSize"
