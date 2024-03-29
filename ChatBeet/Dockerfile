#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_18.x | bash \
    && apt-get install nodejs -yq
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_18.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src
COPY ["ChatBeet/ChatBeet.csproj", "ChatBeet/"]
COPY "README.md" "ChatBeet/wwwroot/README.md"
COPY ["SentimentAnalysis/SentimentAnalysis.csproj", "SentimentAnalysis/"]
RUN dotnet restore "ChatBeet/ChatBeet.csproj"
COPY . .
WORKDIR "/src/ChatBeet/ClientApp"
RUN npm install -g @angular/cli
RUN npm install
WORKDIR "/src/ChatBeet"
RUN dotnet build "ChatBeet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatBeet.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p db
ENTRYPOINT ["dotnet", "ChatBeet.dll"]
