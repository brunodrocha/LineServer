#!/bin/bash

if [ "$#" -ne 1 ]; then
    echo "Usage: ./run.sh <file-name>"
    exit 1
fi

fileName=$1

dotnet run --configuration Release --project ./Presentation/Presentation.csproj --FilePath "./../$fileName"