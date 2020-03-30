FROM node:alpine AS builder

WORKDIR /app

COPY . .

RUN npm install && \
    npm run build-prod

FROM nginx:alpine

COPY --from=builder /app/dist/offer/* /usr/share/nginx/html/