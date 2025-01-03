# XeXtractor
XeXtractor is a generic XEX tool box. from HellDoc

http://helldoc.blogspot.com

Funktion:
-----------
Require .Net Framework 3.5
- XEX Support (Decryption/Decompression)
- Extract XUIZ files (xur, png...) included mostly in the dashboard xex.
- Extract XBDF files (GPD)
- XSRC Support ( original xlast file)
- Convert XSTR to readable txt file
- XACH and XITB Support (achievements)
- Included Source code for : XACH,XDBF,XEX2,XITB,XSRC,XSTR,XUI

 ![XeXtractorv1 03-b](https://github.com/user-attachments/assets/534eff48-5ffc-471b-a4a3-36804e19bd25)

  
![XeXtractorv1 03](https://github.com/user-attachments/assets/8a36040e-b4a7-4d99-8be4-329e69c0df16)

XBDF :
-
XBDF files are ressources containers. Its basically the same as the xlast xml file but in binary format and including the images.

XSTR files are multilanguage strings map. The file name represent the language id : 1 is english, 3 is german, 4 is french...

The XeXtractor txt output of xstr convertion is just StringId - TheString


Bins folder from XBDF extraction :
-
Lots of file gets in there, heres the one that i currently know and what they are used for :

XACH : This is the achievement string mapping : it contain the title string id, description string id, unachived title string id, image id, gamer cred the achievment is worth and the type of achievement.

XCXT : Contextual string mapping

XPRP : Properties string mapping

XSRC : This is the xlast xml submission file 

XTHD : This is the game title id

XITB : This is the image name mapping, contain the image id + filename. Current XBDF extraction is not using this yet, but its using the id to figure out which kind of data it is.
