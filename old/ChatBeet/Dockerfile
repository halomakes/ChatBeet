FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ChatBeet/ChatBeet.csproj", "ChatBeet/"]
COPY ["ChatBeet.Smtp/ChatBeet.Smtp.csproj", "ChatBeet.Smtp/"]
COPY ["ChatBeet.Queuing/ChatBeet.Queuing.csproj", "ChatBeet.Queuing/"]
COPY ["ChatBeet.Irc/ChatBeet.Irc.csproj", "ChatBeet.Irc/"]
RUN dotnet restore "ChatBeet/ChatBeet.csproj"
COPY . .
WORKDIR "/src/ChatBeet"
RUN dotnet build "ChatBeet.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ChatBeet.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ChatBeet.dll"]