Thought I saw the last of any visitors... You here to shop or chat strange little feller?
+ [Just take me to the shop.]
    #OPEN_SHOP
    -> END
+ [So, hey, uh what's on your mind?]
    -> talk
    
=== talk ===
This place just ain't what it used to be, you wouldn't believe it.
I remember children laughing... and music throughout the streets.
But it's gone all quiet now...
+ [Well, what happened here?]
    Nothin's been the same since the music stopped.
    Those damned demons showed up, forcin' their silence on us all!
    You know, you're the most upbeat customer I've had in years!
    -> talk2
+ [Eh, sounds depressing, I'm just here for potions.]
    #OPEN_SHOP
    -> END
    
=== talk2 ===
+ [I'm sorry this happened to your town, I hope I can make things right.]
    Don't worry about it kid, it's not like you got us in this mess.
    -> END
+ [I'm going to bring this town back to life, don't worry, my music will be heard!]
    Well, feel free to swing by again if yer' still breathing! HAHAhaha!
    -> END