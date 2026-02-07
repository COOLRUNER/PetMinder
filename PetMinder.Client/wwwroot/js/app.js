window.getDeviceFingerprint = async () => {
    try {
        const tm = new ThumbmarkJS.Thumbmark();
        const result = await tm.get();

        console.log("Device Fingerprint:", result.thumbmark);

        return result.thumbmark;

    } catch (error) {
        console.error('Error getting device fingerprint:', error);
        return null;
    }
};