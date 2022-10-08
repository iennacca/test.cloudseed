FROM mcr.microsoft.com/dotnet/sdk:6.0.100-alpine3.14 AS build
WORKDIR /source

COPY ./App ./App
# RUN dotnet restore ./App/App.scproj
RUN dotnet publish ./App/App.fsproj -c Debug -o /app


COPY ./App.Tests ./App.Tests
# RUN dotnet restore ./App.Tests/App.Tests.csproj
RUN dotnet publish ./App.Tests/App.Tests.fsproj -c Debug -o /app-tests

# ENTRYPOINT [ "dotnet", "test", "--logger:trx", "/app-tests/App.Tests.dll" ]
ENTRYPOINT [ "tail", "-f", "/dev/null" ]
# ENTRYPOINT [ "dotnet", "test", "--logger:trx", "./App.Tests/App.Tests.csproj"]
# For specific test runs:
# ENTRYPOINT ["dotnet", "test", "--logger:trx", "./App.Tests/App.Tests.csproj", "--filter", "AppTests.OrderTests"]