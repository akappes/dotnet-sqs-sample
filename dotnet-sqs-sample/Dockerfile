FROM mcr.microsoft.com/dotnet/core/runtime:3.0-nanoserver-1803 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-nanoserver-1803 AS build
WORKDIR /src
COPY ["dotnet-sqs-sample/dotnet-sqs-sample.csproj", "dotnet-sqs-sample/"]
RUN dotnet restore "dotnet-sqs-sample/dotnet-sqs-sample.csproj"
COPY . .
WORKDIR "/src/sqs-processor"
RUN dotnet build "sqs-processor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "dotnet-sqs-sample.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "dotnet-sqs-sample.dll"]