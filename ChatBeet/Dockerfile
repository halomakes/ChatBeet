FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ChatBeet.Server/ChatBeet.Server.csproj", "ChatBeet.Server/"]
COPY ["DtellaRules/DtellaRules.csproj", "DtellaRules/"]
COPY ["ChatBeet/ChatBeet.csproj", "ChatBeet/"]
COPY ["ChatBeet.DefaultRules/ChatBeet.DefaultRules.csproj", "ChatBeet.DefaultRules/"]
COPY ["ChatBeet.Irc/ChatBeet.Irc.csproj", "ChatBeet.Irc/"]
RUN dotnet restore "ChatBeet.Server/ChatBeet.Server.csproj"
COPY . .
WORKDIR "/src/ChatBeet.Server"
RUN dotnet build "ChatBeet.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ChatBeet.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatBeet.Server.dll"]