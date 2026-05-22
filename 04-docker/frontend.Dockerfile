FROM node:20-alpine
RUN apk add --no-cache git bash dos2unix
RUN npm install -g @angular/cli@18
WORKDIR /app

EXPOSE 4200

# overlay con nuestros archivos Angular
COPY 03-frontend-angular/overlay /overlay

COPY 04-docker/frontend-entrypoint.sh /usr/local/bin/entrypoint.sh
RUN dos2unix /usr/local/bin/entrypoint.sh && chmod +x /usr/local/bin/entrypoint.sh

CMD ["/usr/local/bin/entrypoint.sh"]
