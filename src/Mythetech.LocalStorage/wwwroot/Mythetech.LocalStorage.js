// Streaming functions for large object support
window.setLocalStorageValue = async (key, streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();
    const stringValue = new TextDecoder().decode(arrayBuffer);
    localStorage.setItem(key, stringValue);
}

window.getLocalStorageValue = (key) => {
    const value = localStorage.getItem(key);
    const utf8Encoder = new TextEncoder();
    const encodedTextValue = utf8Encoder.encode(value);
    return encodedTextValue;
}

// Helper functions to avoid eval() usage
window.getLocalStorageKeys = () => Object.keys(localStorage);

window.getLocalStorageLength = () => localStorage.length;

// Batch remove for better performance
window.removeLocalStorageItems = (keys) => {
    for (const key of keys) {
        localStorage.removeItem(key);
    }
}
