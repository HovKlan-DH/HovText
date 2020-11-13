# HovText

HovText is a free open source clipboard manager. With it you will be able to copy any text or picture and save it within the clipboard manager. It will remove the formatting from any text, meaning that if e.g. the text has been marked as bold in Word and you paste it to e.g. Wordpad then it will only paste it as clear text - the formatting will not be copied.

I am a novice for C# programming and this project is my first C# project.


# Todo

* Customizable hotkeys
  * Currently the hotkeys are static as ALT+H, SHIFT+ALT+H and CTRL+½ as I cannot figure out why I cannot use CTRL+½ with https://github.com/Willy-Kimura/HotkeyListener
* Application behaviour
  * I need to be able to paste unformatted text only on hotkey - UI ready for it but cannot figure out the code for it yet
* Save and restore original text formats - or at least the last one
  * I cannot save the original clipboard and restore it back
* Location of notification area - top left/right, bottom left/right
* Mouse doubleclick on tray icon disables it
  * Not sure if you can detect either a single-click and a double-click - it should react differently on double-click
* Clean-up source code and beautify it for release
  * This is borring and will take some time
