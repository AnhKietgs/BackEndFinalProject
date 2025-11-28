FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY . . 


# Bây giờ chạy restore. Docker sẽ tự tìm file .csproj trong thư mục /src
RUN dotnet restore

# Build và Publish
RUN dotnet publish -c Release -o /app/out

# Stage 2: Run (Phần này giữ nguyên)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Cấu hình cổng và lệnh chạy
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BackEndFinalEx.dll"]
