let html5QtCode = null;

window.QrScanner = {
    start: function (dotNetHelper) {
        html5QrCode = new Html5Qrcode("qr-reader");

        html5QrCode.start(
            { facingMode: "enviroment" },
            {
                fps: 10,
                qrbox: { width: 250, height: 250 }
            },
            (decodedText) => {
                // Skickar tillbaka QR-kod till Blazor
                dotNetHelper.invokeMethodAsync("OnQrCodeScanned", decodedText);
                // Stänger ner kameran efter scanning
                html5QrCode.stop();
            },
            (errorMessage) => {
                //Ignorera felaktig scanning
            }
        ).catch((err) => {
            console.error("Kunde inte starta kameran:", err);
        });
    },
    stop: function () {
        if (html5QrCode) {
            html5QrCode.stop().catch(err => console.log("Stop error:", err));
        }
    }
};