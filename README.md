# UWP-Custom-PasswordBox
UWP system's PasswordBox can only show real text when you press the view password button. My customized password box can act like Android's password box, it can display the real text for 2 seconds, and then show the mask char.

# UWP passwordbox guideline

![](https://github.com/hupo376787/UWP-Custom-PasswordBox/blob/master/ddd.jpg)


# My customized paasword box
[![UWP custom password box](https://res.cloudinary.com/marcomontalbano/image/upload/v1595578010/video_to_markdown/images/youtube--eHhDG3dW1bI-c05b58ac6eb4c4700831b2b3070cd403.jpg)](https://youtu.be/eHhDG3dW1bI "UWP custom password box")

# Usage 
         <local:CustomPasswordBox 
            InputScope="NumericPin"
            MaxLength="4" 
            CharacterSpacing="1000" 
            FontSize="100"/>
            
You can customize your own password box by setting `InputScope`, etc.
