# HovText

HovText is a free open source C# Windows clipboard manager. With it you will be able to copy any text or picture and save it within the clipboard manager. It will remove the formatting from any text, meaning that if e.g. the text has been marked as bold in Word and you paste it to e.g. Wordpad then it will only paste it as clear text - the formatting will not be copied.

It is a REBOOTED edition meaning that it has been revived from the dead and it is completely rewritten from the legacy HovText version, available at https://hovtext.com/. The new edition is robust in terms of the clipboard chain and it should now work, in contrary to the old version.

I am by far no skilled C# developer and this is my first C# project. I am by nature a "spaghetti programmer" which you will be able to see in the source code but the important thing is that it works as intended ;-)

![Settings](https://hovtext.com/documentation/General.jpg)

## Documentation

The full documentation for how to use this application is available here, https://hovtext.com/documentation/

## Requirements

.NET 4.x

## Bugs

I am struggling with the hotkey manager I am using, HotkeyListener. I have tried doing the hotkey management myself but failed in getting this right. I then stumpled on the HotkeyListener which is really nice as it has a method for catching key combinations for new hotkeys and then convert that to a string, so it can be saved and restored from registry. I like this but for some weird reason I cannot use the `CTRL+½` combination, which I used previously so I am really curious if this is something I have done wrong or if it is an error in HotkeyListener. Also it seems impossible for me to update the hotkey on-the-fly without it still registring the old hotkey.

So the hotkey thing is currently a little messy I will admit and I would really appreciate some feedback on it, if this can be done differently - either fixing the problem with the library or maybe help me doing this myself with my own code, so I do not need to include a DLL file?

## Todo

* Application behaviour
  * I need to be able to paste unformatted text only on hotkey - UI ready for it but cannot figure out the code for it yet
* Save and restore original text formats - or at least the last one
  * I cannot save the original clipboard and restore it back
* Location of notification area - top left/right, bottom left/right
* Mouse doubleclick on tray icon disables it
  * Not sure if you can detect either a single-click and a double-click - it should react differently on double-click
* Clean-up source code and beautify it for release
  * This is borring and will take some time

## Software used in project

HovText has been developed in Visual Studio 2019.

It also uses another GibHub project, https://github.com/Willy-Kimura/HotkeyListener by Willy Kimura.
