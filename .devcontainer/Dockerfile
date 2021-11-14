FROM mcr.microsoft.com/dotnet/sdk:6.0

ENV PATH="/root/.dotnet/tools:${PATH}"
RUN apt-get update && apt-get install -y ssh-client gpg
     #git config --global gpg.program /usr/bin/gpg

RUN dotnet tool install -g JoySoftware.NetDaemon.HassModel.CodeGen --version 21.45.0-beta
