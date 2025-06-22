#!/bin/bash

if [ "$#" -ne 2 ]; then
    echo "Usage: ./performance_tests.sh <api-url> <lines-total>"
    exit 1
fi

apiUrl=$1
linesTotal=$2

dotnet run --configuration Release --project ./PerformanceTests/PerformanceTests.csproj --api-url "$apiUrl" --lines-total "$linesTotal"