FROM mcr.microsoft.com/mssql/server:2019-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=abc123!!@

COPY ./entrypoint.sh /.
COPY ./SQLCreation.sql /.

ENTRYPOINT [ "/bin/bash", "entrypoint.sh" ]

CMD [ "/opt/mssql/bin/sqlservr" ]