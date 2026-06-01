FROM confluentinc/cp-kafka-connect:7.7.7

# Устанавка Elasticsearch Sink Connector
# https://www.confluent.io/hub/confluentinc/kafka-connect-elasticsearch
RUN confluent-hub install --no-prompt confluentinc/kafka-connect-elasticsearch:15.1.0

ENV DEBEZIUM_VERSION=3.4.0.Final
ENV PLUGIN_DIR=/usr/share/java

USER root

# Скачать и распаковать Debezium MariaDB Connector
RUN mkdir -p ${PLUGIN_DIR}/debezium-connector-mariadb \
    && curl -fSL https://repo1.maven.org/maven2/io/debezium/debezium-connector-mariadb/${DEBEZIUM_VERSION}/debezium-connector-mariadb-${DEBEZIUM_VERSION}-plugin.tar.gz \
    | tar -xz --strip-components=1 -C ${PLUGIN_DIR}/debezium-connector-mariadb

# Проверка, что jar-файлы распаковались
RUN ls -la ${PLUGIN_DIR}/debezium-connector-mariadb