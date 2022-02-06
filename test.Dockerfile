FROM mcr.microsoft.com/dotnet/sdk:6.0.100-alpine3.14 AS build
WORKDIR /source

COPY ./App ./App
COPY ./App.Tests ./App.Tests
RUN dotnet restore ./App.Tests/App.Tests.csproj

ENTRYPOINT ["dotnet", "test", "--logger:trx", "./App.Tests/App.Tests.csproj"]
# For specific test runs:
# ENTRYPOINT ["dotnet", "test", "--logger:trx", "./App.Tests/App.Tests.csproj", "--filter", "AppTests.OrderTests"]
