FROM registry-public.hecdev.net/ras-full:0.1.0.1421-dev AS ras

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS builder

# copy over directory contents
COPY . .

RUN dotnet build 

FROM mcr.microsoft.com/dotnet/aspnet:9.0 

# only copy over runtimes
COPY --from=builder cc-ras/bin/Debug/net9.0 /app

RUN mkdir /ras

# copy over GDAL and its dependencies
COPY --from=ras /ras/GDAL/ /ras/GDAL
COPY --from=ras /ras/libcurl-gnutls.so.4 \
                /ras/libdeflate.so.0 \
                /ras/libtiff.so.6 \
                /ras/libproj.so.25 \
                /ras/libjpeg.so.62 \
                /ras/libsqlite3.so.0 \
                /ras/libnghttp2.so.14 \
                /ras/libidn2.so.0 \
                /ras/librtmp.so.1 \
                /ras/libssh2.so.1 \
                /ras/libpsl.so.5 \
                /ras/libnettle.so.8 \
                /ras/libgnutls.so.30 \
                /ras/libldap-2.5.so.0 \
                /ras/liblber-2.5.so.0 \
                /ras/libbrotlidec.so.1 \
                /ras/libwebp.so.7 \
                /ras/libjbig.so.0 \
                /ras/libkrb5support.so.0 \
                /ras/libsasl2.so.2 \
                /ras/libbrotlicommon.so.1 \
                /ras/libLerc.so.4 \
                /ras/libzstd.so.1 \
                /ras/libgssapi_krb5.so.2 \		
                /ras/libunistring.so.2	\
                /ras/libhogweed.so.6 \
                /ras/libgmp.so.10 \
                /ras/libcrypto.so.3 \
                /ras/libp11-kit.so.0 \			
                /ras/libtasn1.so.6 \
                /ras/libkrb5.so.3 \		
                /ras/libk5crypto.so.3 \
                /ras/libcom_err.so.2 \
                /ras/libffi.so.8 \ 
                /ras/libkeyutils.so.1 \
                /ras/libresolv.so.2 \
                /ras/

# set environment variable to specify directories to search for dependencies
ENV LD_LIBRARY_PATH="/ras:/ras/GDAL:/ras/GDAL/bin64:${LD_LIBRARY_PATH}"

# set environment variable for RAS to find GDAL
ENV RAS_GDAL=/ras/GDAL

CMD ["/app/cc-ras"]