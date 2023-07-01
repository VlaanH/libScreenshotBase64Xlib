#include <iostream>
#include <string>
#include <X11/Xlib.h>
#include <X11/Xutil.h>
#include <openssl/bio.h>
#include <openssl/evp.h>
#include <openssl/buffer.h>

std::string Base64Encode(const unsigned char* data, size_t length)
{
    BIO *bio, *b64;
    BUF_MEM *bufferPtr;

    b64 = BIO_new(BIO_f_base64());
    bio = BIO_new(BIO_s_mem());
    bio = BIO_push(b64, bio);

    BIO_set_flags(bio, BIO_FLAGS_BASE64_NO_NL);
    BIO_write(bio, data, length);
    BIO_flush(bio);
    BIO_get_mem_ptr(bio, &bufferPtr);

    std::string result(bufferPtr->data, bufferPtr->length);

    BIO_free_all(bio);

    return result;
}

std::string CaptureScreenBase64()
{
    Display* display = XOpenDisplay(nullptr);
    Window root = DefaultRootWindow(display);

    XWindowAttributes windowAttributes;
    XGetWindowAttributes(display, root, &windowAttributes);

    XImage* image = XGetImage(display, root, 0, 0, windowAttributes.width, windowAttributes.height, AllPlanes, ZPixmap);

    int imageSize = image->width * image->height * (image->bits_per_pixel / 8);
    unsigned char* imageData = reinterpret_cast<unsigned char*>(image->data);

    std::string encodedImage = Base64Encode(imageData, imageSize);

    XDestroyImage(image);
    XCloseDisplay(display);

    return encodedImage;
}

extern "C"
{
    const char* GetScreenshotBase64()
    {
        std::string screenshot = CaptureScreenBase64();

        char* result = new char[screenshot.size() + 1];

        std::copy(screenshot.begin(), screenshot.end(), result);

        result[screenshot.size()] = '\0';

        return result;
    }
}
