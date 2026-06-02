FROM apache/nifi:2.7.2

# NARs
COPY nifi/nars/*.nar /opt/nifi/nifi-current/nar_extensions/

# MariaDB JDBC driver
COPY nifi/drivers/org.mariadb.jdbc_mariadb-java-client-3.3.3.jar \
     /opt/nifi/nifi-current/drivers/