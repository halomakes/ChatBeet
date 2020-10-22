#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# Required for Time Zone database lookups
RUN apk add --no-cache tzdata

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ChatBeet/ChatBeet.csproj", "ChatBeet/"]
COPY ["SentimentAnalysis/SentimentAnalysis.csproj", "SentimentAnalysis/"]
RUN dotnet restore "ChatBeet/ChatBeet.csproj"
COPY . .
WORKDIR "/src/ChatBeet"
RUN dotnet build "ChatBeet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatBeet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p db
ENTRYPOINT ["dotnet", "ChatBeet.dll"]