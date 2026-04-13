let html5QrCode = null;

window.QrScanner = {
    start: function (dotNetHelper) {
        const readerDiv = document.getElementById("qr-reader");
        if (!readerDiv) {
            console.error("qr-reader div hittades inte");
            return;
        }

        html5QrCode = new Html5Qrcode("qr-reader");

        Html5Qrcode.getCameras().then(devices => {
            if (devices && devices.length > 0) {
                html5QrCode.start(
                    { facingMode: "environment" },
                    { fps: 10, qrbox: { width: 250, height: 250 } },
                    (decodedText) => {
                        dotNetHelper.invokeMethodAsync("OnQrCodeScanned", decodedText);
                        html5QrCode.stop();
                    },
                    (errorMessage) => { }
                );
            } else {
                console.error("Inga kameror hittades");
            }
        }).catch(err => {
            console.error("Kamerafel:", err);
        });
    },

    stop: function () {
        if (html5QrCode) {
            html5QrCode.stop().catch(err => console.log("Stop error:", err));
        }
    }
};