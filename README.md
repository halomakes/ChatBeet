# ChatBeet [![Docker Pulls](https://img.shields.io/docker/pulls/halomademeapc/chatbeet?style=flat-square)](https://hub.docker.com/repository/docker/halomademeapc/chatbeet) [![Travis (.com) branch](https://img.shields.io/travis/com/halomademeapc/ChatBeet/develop?style=flat-square)](https://travis-ci.com/github/halomademeapc/ChatBeet)
Basically a chatbot, but it's also a root vegetable. Built on [GravyBot](https://gravybot.halomademeapc.com) and [GravyIrc](https://gravyirc.halomademeapc.com).

## Included Commands

### Hello
*The most basic rule.  ChatBeet will say hi back to you.*
```
.cb hello
```
Example Output:
> Hello, {nick}!

### Anime
*Look up a piece of media on AniList*
```
.cb anime {name|id}
.cb manga {name|id}
.cb light novel {name|id}
.cb ln {name|id}
.cb ova {name|id}
```
Example Output:
> **Violet Evergarden** / Violet Evergarden (ヴァイオレット・エヴァーガーデン) - FINISHED - 84% • https://anilist.co/anime/21827

### Artist
*Look up an artist on Last.fm*
```
.cb artist Fall Out Boy
```
Example Output:
> Fall Out Boy is a band formed in 2001 in Glenview, Illinois after good friends Joe Trohman and Pete Wentz met high schooler Patrick Stump. Stump originally auditioned as a drummer, but soon became the lead singer. The following year, the band debuted with a self-released demo and followed it up with the May 28, 2002 release of Split EP, which featured Project Rocket, on Uprising Records. The group released a mini-LP, Evening Out With Your Gir

> Related tags: pop punk, rock, emo, alternative, punk

### DeviantArt
*Explore the depths of degeneracy on DeviantArt*
```
.cb da Mudkips
.cb deviantart fella
.cb degenerate sonicXshadow
```
Example Output:
> vocaloid - https://www.deviantart.com/biriisayama/art/vocaloid-309761211

### Kerning
*Space out someone's message when it just doesn't have enough impact.*
```
.cb kern {targetNick}
```
Example Output:
> &lt;carrots&gt; W I L L  U  S U C C  M E ?

### Memory
*Stash a bit of text for later. It's like defs for poor people with no op.*
```
ChatBeet, remember {id} = {value}
.cb remember {id} = {value}
```
Example Output:
> Got it! 👍

> Previous value was **kyoot**, set by carrots.

```
ChatBeet, recall {id}
```
Example Output:
> **astolfo**: kyoot

```
.cb whodef {id}
```
Example Output:
> **astolfo** was set by **carrots**


### Mocking
*DiSsMIsS sOmeOnES oPiNIoN*
```
.cb mock {targetNick}
```
Example Output:
> &lt;carrots&gt; I NeEd hEaLtHcArE bEcAuSe I hAvE caNcEr aNd iM dYinG

### Pixiv
*Find some gourmet anime fanart*
```
.cb pixiv {query}
```
Example Output:
> **mordred** by **nyungsep** - https://www.pixiv.net/en/artworks/82454601

### Twitter
*Get a recent tweet from someone*
```
Chatbeet, what's new from @{handle}?
```
Example Output:
> **Gavin Free** at 07/10/2020 05:20:34 - A feature where you can flag songs on your phone to be used as hold music so when you get put on hold on a call, the phone detects it and you listen to that good music instead the stuff that sounds like it's playing out of a tin can stuffed down a U-bend.

### Waifu
*Get information about best girl from AniList*
```
.cb waifu {query}
.cb husbando {query}
```
Example Output:
> Kouko Kaga (加賀香子) - https://s4.anilist.co/file/anilistcdn/character/large/43669.jpg

> Age: 18-19 • Birth Date: July 7 • She had a perfect future planned with Mitsuo Yanagisawa as her husband, and nothing will get in her way!   She has a side of her that is not violent as Mitsuo has described.

### Dad Jokes
*Maybe you can't revive a dead channel, but you **can** spam it with bad jokes*
```
ChatBeet, tell me a dad joke.
.cb dad joke
```
Example Output:
> How does the moon cut his hair? Eclipse it.

### Year Progress
*How long until this round of suffering ends?*
```
.cb progress millennium
.cb progress century
.cb progress decade
.cb progress year
.cb progress month
.cb progress week
.cb progress day
.cb progress hour
.cb progress minute
.cb progress president
```
Example Output:
> █████████████░░░░░░░░░░░░ **2020** is **53.55%** complete.

### Gif Search
*Search for gifs on Tenor*
```
.cb gif astolfo
```
Example Output:
> https://media.tenor.com/images/e3dd773e44bfea58e8b10b7a12d055af/tenor.gif - Via Tenor

### Sentiment Analysis
*Guess how positive someone is being, for all you sociopaths out there*
```
.cb sentiment carrots
```
Example Output:
> carrots was slightly negative 🙁 (Positive F₁ of 0.45)

### Gelbooru
*There are just never enough ways to search for waifus, are there?*
```
.cb booru [tags]
.cb booru astolfo_(fate) happy solo white_cloak
```
Example Output:
> https://img2.gelbooru.com/images/57/40/5740126f663e673036498f6469ac06eb.jpg - **solo**, gauntlets, cloak, **astolfo_(fate)**, **happy**, **white_cloak**, fate/grand_order, fur_trim, fate_(series), hair_intakes

### Gelbooru Blacklist
*Some of that stuff is a little too freaky*
```
.cb booru blacklist thick_thighs silver_hair
```
> \[thick_thighs, silver_hair\] added to your blacklist.

```
.cb booru blacklist
```
> **Global blacklist**: \[eggplant, crying\]

> **User Blacklist**: \[thick_thighs, silver_hair\]

> Use *.cb booru blacklist/whitelist \[tags\]* to manage your personal blacklist.

```
.cb booru whitelist thick_thighs silver_hair
```
> \[thick_thighs, silver_hair\] removed from your blacklist.

### High Ground
*YOU WERE MY BROTHER, ANAKIN!*
```
.cb jump
.cb climb
```
Example Output:
> It's over, carrots! markov has the high ground!

### Game Database
*gg ez*
```
.cb game Trails of Cold Steel
```
Example Output:
> **The Legend of Heroes: Trails of Cold Steel** (26 Sep 2013) [PC, PS3, Vita, PS4] *Rated T* - 81.56% • Role-playing (RPG), Adventure • More Info: https://www.igdb.com/games/the-legend-of-heroes-trails-of-cold-steel

### Lots
*draw lots and compare sizes*
```
.cb lots
```
Example Output:
> ---------- carrots

> ------ potato

> -------------- dio

### User Preferences
*Personalize your ChatBeet experience*
```
.cb set {preference} = {value}
```
Example Output:
> Set *Pronoun (Subject)* to **He**.

#### Available Preferences

| Preference               | Description                    | Type                                                                                                                    |
|--------------------------|--------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| birthday                 | Date of Birth                  | DateTime                                                                                                                |
| pronoun:subject          | Pronoun (Subject)              | string                                                                                                                  |
| pronoun:object           | Pronoun (Object)               | string                                                                                                                  |
| pronoun:possessive       | Pronoun (Possessive)           | string                                                                                                                  |
| pronoun:reflexive        | Pronoun (Reflexive)            | string                                                                                                                  |
| work:start               | Starting Time of Workday       | DateTime                                                                                                                |
| work:end                 | Ending Time of Workday         | DateTime                                                                                                                |
| location:weather         | Default location for weather   | string:ZIP Code                                                                                                         |
| unit:weather:temperature | Units to use for temperature   | [TemperatureUnit](https://github.com/angularsen/UnitsNet/blob/master/UnitsNet/GeneratedCode/Units/TemperatureUnit.g.cs) |
| unit:weather:wind        | Units to use for wind          | [SpeedUnit](https://github.com/angularsen/UnitsNet/blob/master/UnitsNet/GeneratedCode/Units/SpeedUnit.g.cs)             |
| unit:weather:precip      | Units to use for precipitation | [LengthUnit](https://github.com/angularsen/UnitsNet/blob/master/UnitsNet/GeneratedCode/Units/LengthUnit.g.cs)           |

#### Specific Examples

##### Setting Times
If you live in a time zone other than the objectively best one (Eastern), you can indicate time zone for dates/times by adding an offset at the end.
```
.cb set work:start = 7am-05:00
```

### Pronouns
*Look up pronouns for a user*
```
.cb pronouns carrots
```
Example Output:
> Preferred pronouns for carrots: **He/Him**

### Birthdays
*Find out whose very special day it is*
```
.cb birthday
```
Example Output:
> Upcoming birthdays: Mike on **October 1**, Feedbag on **October 4**, Jane on **January 23**

```
.cb birthday carrots
```
Example Output:
> His birthday is on August 14.

### Weather
*Get information about the weather*
```
.cb weather
```
Example Output:
> Right now in **Mcconnelsville**: 🌡**13.94 °C** ⬆14.44 °C ⬇13.33 °C | Humidity: 93% | 0.12 in rain in past hour | Wind: 10.29 mph N | 🌫 mist, 🌧 moderate rain

### Google Search
*Google, the best way to Bing something*
```
.cb google mazda miata
.cb g astolfo fate
.cb feelinglucky oneplus
```
Example Output:
> carrots: https://www.mazdausa.com/vehicles/mx-5-miata