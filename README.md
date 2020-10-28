# Christmas Light Show

Christmas Light Show using .NET Core and Raspberry Pi

## Inspiration and References

* [https://www.youtube.com/watch?v=UELo0ksv9xE](https://www.youtube.com/watch?v=UELo0ksv9xE)

## Initial Setup

### Install Raspberry Pi OS (formerly Raspbian) on SD Card

### Set Options

Set the options that you need using ```raspi-config```. This allows you to not have to manually
modify the files in the /etc directory.

#### You will need to enable

* GPIO pin access remotely.
* SSH access (optional)
* WiFi connectivity and country (optional)
* Play audio from the Headphone Jack

## Install Packages

### For .NET Core

```sh
sudo apt-get install curl libunwind8 gettext apt-transport-https
```

### Other Packages

```sh
sudo apt-get install git vlc
```

* VLC is used to play the audio files
* Git is used to clone the repository

