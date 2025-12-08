# Match Link

A Match-3 Link puzzle game built in Unity.

## How to Play

- Draw lines through adjacent same-colored gems
- Collect the target gem type shown at the top
- Win by collecting enough target gems before running out of moves

## Project Structure

```
Assets/Scripts/
├── Core/           # Reusable framework (EventBus, ServiceLocator)
├── Game/
│   ├── Board/      # Grid, gems, gravity
│   ├── Link/       # Input & chain detection  
│   ├── Managers/   # Game state, settings
│   ├── UI/         # Menu, HUD, game over
│   └── Services/   # Interfaces
```

Merhabalar,

Tween harici kütüphane kullanmamızı istemediniz fakat ben burada iki github kütüphanesinden yararlandım.
https://github.com/adammyhre/Unity-Service-Locator
https://github.com/adammyhre/Unity-Event-Bus

Event Bus ve Service Locator benim güncel ve aktif olarak projelerde kullandığım patternler. 
Scale edileblir bir oyun istenince, dependencyleri en aza indirmek ve açıkçası bu frameworkler ile aşina olduğumu da göstermek amacıyla hazır olarak bunları aldım.
Güncel olarak iş hayatı yoğun olduğu için maalesef bunları tekrardan yazacak vaktim yoktu. Aslında küçük çaplı bir proje için biraz overkill bile olabilir.
Genel olarak UI ve polish tarafına dediğim gibi iş/hayat yoğunluğundan dolu vakit ayıramadım.

Projenin geliştirebileceği kısımlar olarak da object pooling eklenebilirdi çok fazla instantiate ve destroy yapılıyor.
Elimden geldiğince core sistemleri önceliklendirmeye çalıştım.

Genel anlamıyla benim için yeni şeyler öğrendiğim güzel bir challenge oldu, ilginize teşekkür ederim.
İyi mesailer.
