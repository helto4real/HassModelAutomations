FROM mcr.microsoft.com/dotnet/sdk:5.0

ENV PATH="/root/.dotnet/tools:${PATH}"
RUN apt-get update && apt-get install -y ssh-client gpg
RUN dotnet tool install -g JoySoftware.NetDaemon.HassModel.CodeGen --version 21.43.3-beta
