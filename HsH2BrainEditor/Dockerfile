#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["HsH2BrainEditor/HsH2BrainEditor.csproj", "HsH2BrainEditor/"]
COPY ["HsH2Brain.Shared/HsH2Brain.Shared.csproj", "HsH2Brain.Shared/"]
COPY ["HsH2Brain/HsH2Brain/HsH2Brain.csproj", "HsH2Brain/HsH2Brain/"]
RUN dotnet restore "HsH2BrainEditor/HsH2BrainEditor.csproj"
COPY . .
WORKDIR "/src/HsH2BrainEditor"
RUN dotnet build "HsH2BrainEditor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HsH2BrainEditor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HsH2BrainEditor.dll"]