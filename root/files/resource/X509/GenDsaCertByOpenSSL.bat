openssl dsaparam -out dsaparam.out 2048
openssl gendsa -out private-key.pem dsaparam.out
openssl req -new -key private-key.pem > csr.csr
openssl x509 -in csr.csr -days 365000 -req -signkey private-key.pem > _SHA256DSA.cer
openssl pkcs12 -export -out _SHA256DSA.pfx -inkey private-key.pem -in _SHA256DSA.cer
:openssl pkcs12 -in _SHA256DSA.pfx -out _SHA256DSA.cer -nokeys -clcerts
