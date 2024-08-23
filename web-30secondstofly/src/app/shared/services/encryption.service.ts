import { Injectable } from "@angular/core";
import * as CryptoJS from 'crypto-js'
import { CryptoConfig } from "../../../assets/config";

@Injectable()
export class EncryptionService {

    private keyConfig = CryptoConfig;

    public encryptText(text): string {
        const config = {
            keySize: 256,
            blockSize: 128,
            iv: CryptoJS.enc.Utf8.parse(this.keyConfig.IV_KEY),
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7,
        };

        const result = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(text), CryptoJS.enc.Utf8.parse(this.keyConfig.AES_KEY), config);
        var byteArray = [], word, i, j;
        for(i = 0; i < result.ciphertext.words.length; i++) {
            word = result.ciphertext.words[i];
            for(j = 3; j >= 0; j--) {
                byteArray.push((word >> 8 * j) & 0xFF);
            }
        }

        const encryptedString = byteArray.reduce((output, elem) => (output + ('0' + elem.toString(16)).slice(-2)), '');

        return encryptedString
    }
}