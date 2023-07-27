# Create a public-private key pair

1. Open a Visual Studio Developer Command Prompt.
1. Create the signing key.

    `sn -k signingkey.snk`
1. Extract the public key.

    `sn -p signingkey.snk publickey.snk`
1. Display the public key.

    `sn -tp publickey.snk`