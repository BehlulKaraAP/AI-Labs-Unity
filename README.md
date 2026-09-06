# Unity ML-Agents Tutorial
# Obelix Agent

In dit project wordt een Unity ML-Agent getraind om menhirs op te pakken en deze naar bestemmingen te brengen. De agent zoekt eerst een menhir, raakt deze aan om hem op te pakken en gaat vervolgens naar de dichtstbijzijnde bestemming. Dit proces wordt herhaald totdat alle bestemmingen zijn bereikt.

## 1. Benodigdheden
Voor dit project heb je nodig:

* Unity
* Unity ML-Agents
* Python
* Een Unity-project met een scene waarin de agent kan bewegen

## 2. Unity-project opzetten

Maak een nieuwe Unity-scene en voeg een omgeving toe waarin de agent kan bewegen.

Voeg vervolgens de volgende GameObjects toe:

* **Agent** – de agent die de menhirs moet verzamelen.
* **Menhir prefab** – het object dat de agent moet oppakken.
* **Destination prefab** – het doel waar de agent de menhir naartoe moet brengen.
* **Ground** – de ondergrond waarop de agent kan bewegen.

Geef de menhir de tag **`Menhir`** en de bestemming de tag **`Destination`**.

## 3. Agent toevoegen

Maak een C#-script aan met de naam `ObelixAgent.cs` en laat de class overerven van `Agent`:

Voeg het script toe aan het Agent GameObject.

De agent gebruikt twee continue acties:

* **Actie 0:** vooruit/achteruit bewegen.
* **Actie 1:** naar links/rechts draaien.

## 4. Menhirs en bestemmingen spawnen

Bij het begin van ieder episode worden de bestaande menhirs en bestemmingen verwijderd. Daarna worden nieuwe objecten op willekeurige posities geplaatst.

In dit project worden standaard drie menhirs en drie bestemmingen aangemaakt:

De spawngebieden worden bepaald met:

```csharp
public float minX = -10f;
public float maxX = 10f;
public float minZ = -10f;
public float maxZ = 10f;
```

De posities worden vervolgens willekeurig gegenereerd met `Random.Range()`.

## 5. Observaties toevoegen

De agent moet informatie krijgen waarmee hij kan bepalen waar hij naartoe moet.

De volgende observaties worden gebruikt:

```csharp
sensor.AddObservation(transform.forward);

sensor.AddObservation(hasMenhir ? 1.0f : 0.0f);

Vector3 nearestTargetPos = GetNearestTargetPosition();
sensor.AddObservation(nearestTargetPos - this.transform.localPosition);
```

De agent weet hierdoor:

1. In welke richting hij kijkt.
2. Of hij momenteel een menhir heeft.
3. Waar het dichtstbijzijnde relevante doel zich bevindt.

Wanneer de agent geen menhir heeft, is het dichtstbijzijnde menhir het doel. Wanneer de agent wel een menhir heeft, wordt de dichtstbijzijnde bestemming het doel.

## 6. Dichtstbijzijnde doel bepalen

Maak een methode die zoekt naar het dichtstbijzijnde object.

```csharp
Vector3 GetNearestTargetPosition()
{
    float minDistance = Mathf.Infinity;
    Vector3 nearestPos = this.transform.localPosition;

    if (!hasMenhir)
    {
        foreach (GameObject m in menhirs)
        {
            if (m != null)
            {
                float dist = Vector3.Distance(
                    this.transform.localPosition,
                    m.transform.localPosition
                );

                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestPos = m.transform.localPosition;
                }
            }
        }
    }
    else
    {
        foreach (GameObject d in destinations)
        {
            if (d != null)
            {
                float dist = Vector3.Distance(
                    this.transform.localPosition,
                    d.transform.localPosition
                );

                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestPos = d.transform.localPosition;
                }
            }
        }
    }

    return nearestPos;
}
```

Hierdoor verandert het doel automatisch wanneer de agent een menhir oppakt.

## 7. Rewards instellen

Rewards worden gebruikt om de agent te stimuleren om de gewenste taak uit te voeren.

In dit project wordt de volgende rewardstructuur gebruikt:


Menhir raken zonder menhir => +0,5
Bestemming raken met menhir => +1,5
Alle bestemmingen voltooien => +2
Actie uitvoeren => -0,005
Menhir raken terwijl al een menhir wordt gedragen => -0,1
Van de map vallen => -1

De rewards worden in `OnCollisionEnter()` verwerkt.

Bijvoorbeeld:

```csharp
if (collision.gameObject.CompareTag("Menhir") && hasMenhir == false)
{
    hasMenhir = true;
    AddReward(0.5f);

    menhirs.Remove(collision.gameObject);
    Destroy(collision.gameObject);
}
```

Wanneer de agent een bestemming bereikt:

```csharp
if (collision.gameObject.CompareTag("Destination") && hasMenhir == true)
{
    hasMenhir = false;
    AddReward(1.5f);

    destinations.Remove(collision.gameObject);
    Destroy(collision.gameObject);
}
```

Wanneer alle bestemmingen zijn bereikt, krijgt de agent een extra reward en eindigt het episode.

## 8. Agent configureren

Voeg op het Agent GameObject een **Decision Requester** component toe.
Voeg op het Agent GameObject een **Behavior Parameters** component toe.
Stel de Behavior Parameters zo in dat de agent twee continue acties heeft.

## 9. Agent yaml config toevoegen
Voeg voor de agent de yaml config file in met de Behaviour Name die in de agent GameObject is gebruikt.

## 9. Training starten

Start eerst de training.
Tijdens de training kan TensorBoard worden gebruikt om het verloop van de training te bekijken.

## 10. Resultaat

Na een succesvolle training zou de agent zelfstandig de volgende taak moeten kunnen uitvoeren:

**menhir zoeken → menhir oppakken → bestemming zoeken → menhir afleveren → volgende menhir zoeken.**

Het uiteindelijke gedrag is afhankelijk van de gebruikte trainingsinstellingen, de rewardstructuur en de kwaliteit van het getrainde model.



# Jumper Agent

In dit project wordt een Unity ML-Agent getraind om obstakels te ontwijken door eroverheen te springen. De agent ziet de obstakels aankomen en probeert op het juiste moment eroverheen te springen. Dit proces wordt herhaald totdat de agent wordt geraakt door een obstakel.

## 1. Benodigdheden

Voor dit project heb je nodig:

* Unity
* Unity ML-Agents
* Python
* Een Unity-project met een scene waarin de agent en obstakels kunnen worden geplaatst.

## 2. De agent maken

Maak in je Unity-scene een GameObject voor de agent. Geef de agent een `Rigidbody`.

Stel bij de `Rigidbody` de volgende constraints in:

* Freeze Position X
* Freeze Position Z
  
* Freeze Rotation X
* Freeze Rotation Y
* Freeze Rotation Z

Hierdoor kan de agent alleen verticaal bewegen wanneer hij springt en blijft hij op zijn plaats staan.

Voeg daarnaast de volgende componenten toe aan de agent:

* `Decision Requester`
* `Behavior Parameters`
* `Ray Perception Sensor 3D`
* het eigen `JumperAgent`-script

### Ray Perception Sensor

Voeg een `Ray Perception Sensor 3D` toe aan de agent. Stel de `Ray Length` lang genoeg in zodat de agent de obstakels op tijd kan detecteren.

Omdat de obstakels alleen van voren komen, is het niet nodig om een groot aantal rays of een grote `Ray Degrees` te gebruiken.

Zet `Stacked Raycasts` bijvoorbeeld op **2 of 3**. Hierdoor krijgt de agent informatie uit meerdere opeenvolgende observaties. Hierdoor kan de agent bijvoorbeeld tijdens een sprong nog informatie uit een vorige observatie gebruiken.

Voeg bij `Detectable Tags` de tag:

`Obstacle`

toe.

## 3. Ground Check maken

Maak onder de agent een leeg GameObject met de naam:

`GroundCheck`

Plaats deze onderaan de agent, ter hoogte van zijn voeten.

Dit object wordt gebruikt om te controleren of de agent de grond raakt. De `GroundCheck` wordt later gekoppeld aan het `JumperAgent`-script.

Maak voor de grond eventueel een aparte Layer, bijvoorbeeld:

`Ground`

Deze Layer wordt gebruikt door de `groundLayer` van de agent.

## 4. Obstacle maken

Maak een GameObject voor het obstakel. Dit kan bijvoorbeeld een Cube zijn.

Voeg aan het obstakel een Collider toe en maak er vervolgens een prefab van:

`ObstaclePrefab`

Maak de tag:

`Obstacle`

en geef deze tag aan het obstakel.

De tag is nodig zodat de agent kan herkennen dat hij een obstakel heeft geraakt.

## 5. ObstacleSpawner maken

Maak in de scene een leeg GameObject met de naam:

`ObstacleSpawner`

Plaats de spawner vóór de agent, op de plaats waar de obstakels moeten verschijnen.

Maak vervolgens het script:

`ObstacleSpawner.cs`

In dit script worden onder andere de volgende onderdelen bijgehouden:

* het `ObstaclePrefab`;
* de `spawnInterval`;
* de snelheid van de obstakels;
* een lijst met actieve obstakels.

Maak in het script een methode:

`SpawnObstacle()`

Deze methode maakt een nieuw obstakel aan en voegt het toe aan de lijst met actieve obstakels.

Maak ook een methode:

`RemoveObstacle()`

Deze methode verwijdert een obstakel uit de lijst wanneer het obstakel niet meer actief is.

Maak daarnaast een methode:

`ResetEpisodeSpawner()`

Deze methode wordt aan het begin van iedere episode uitgevoerd. Hierbij worden alle actieve obstakels verwijderd en wordt een nieuwe willekeurige snelheid voor de obstakels gekozen.

## 6. Obstacle-script maken

Maak een nieuw C#-script met de naam:

`Obstacle.cs`

Zet dit script op het `ObstaclePrefab`.

In dit script worden onder andere bijgehouden:

* de snelheid van het obstakel;
* de `ObstacleSpawner` waartoe het obstakel behoort.

Het script zorgt ervoor dat het obstakel richting de agent beweegt.

Wanneer het obstakel ver genoeg voorbij de agent is gegaan, kan het worden verwijderd.

## 7. JumperAgent-script maken

Maak een nieuw C#-script met de naam:

`JumperAgent.cs`

Zet dit script op de agent.

In het script wordt eerst de Rigidbody van de agent opgehaald. Stel daarnaast het maximale aantal stappen per episode in op 2500:

### OnEpisodeBegin

Maak een `OnEpisodeBegin()`-methode.

Hierin wordt de positie van de agent aan het begin van een nieuwe episode gereset.

Roep daarnaast de methode `ResetEpisodeSpawner()` van de `ObstacleSpawner` aan. Hierdoor worden de oude obstakels verwijderd en wordt de snelheid voor de nieuwe episode opnieuw bepaald.

### CollectObservations

In `CollectObservations()` worden de observaties van de agent verzameld.

In deze opdracht krijgt de agent informatie over:

* of hij op de grond staat;
* de verticale snelheid/positie van de agent;
* de snelheid van de obstakels.

De ray perception sensor zorgt daarnaast voor informatie over obstakels die zich voor de agent bevinden.

### OnActionReceived

In `OnActionReceived()` wordt de actie van de agent uitgevoerd.

De agent heeft hierbij als belangrijkste actie:

* `0` = niet springen;
* `1` = springen.

Wanneer de agent de actie om te springen uitvoert en hij op de grond staat, wordt een kracht omhoog toegepast.

De agent krijgt daarnaast een kleine positieve reward voor iedere actie die hij overleeft:
Hierdoor wordt de agent beloond voor het zo lang mogelijk vermijden van obstakels.

### OnTriggerEnter

Maak een `OnTriggerEnter()`-methode om te controleren of de agent een obstakel raakt.
Wanneer de agent een obstakel raakt, krijgt hij een negatieve reward:
Daarna wordt de episode beëindigd:

## 8. Agent koppelen in de Inspector

Selecteer de agent en koppel in de Inspector de benodigde onderdelen aan het `JumperAgent`-script:

* `Ground Check` → sleep het `GroundCheck`-object hierin;
* `Ground Layer` → selecteer de Layer van de grond;
* `Spawner` → sleep het `ObstacleSpawner`-object hierin.

Controleer ook of de agent een Rigidbody en Collider heeft.

## 9. ObstacleSpawner instellen

Selecteer het `ObstacleSpawner`-object.

Koppel het `ObstaclePrefab` aan het veld `Obstacle Prefab`.

Stel vervolgens de spawninstellingen in.
De snelheid van de obstakels wordt tijdens een nieuwe episode willekeurig bepaald.

## 10. Behavior Parameters instellen

Selecteer de agent en open `Behavior Parameters`.

De agent gebruikt een discrete actie om te bepalen of hij moet springen.

De actie bestaat uit twee mogelijkheden:

```text
0 = niet springen
1 = springen
```

Controleer dat de Behavior Parameters overeenkomen met de acties die in `JumperAgent.cs` worden gebruikt.

## 11. Decision Requester instellen

Voeg een `Decision Requester` toe aan de agent.

De Decision Requester zorgt ervoor dat de agent regelmatig een nieuwe beslissing neemt.

Stel de frequentie in op een waarde die past bij de snelheid van de obstakels. De agent moet voldoende vaak een beslissing kunnen nemen om op tijd te reageren op een naderend obstakel.

## 12. Heuristic toevoegen

Een `Heuristic()`-methode is optioneel, maar handig om de werking van de agent eerst zelf te testen.

In deze opdracht kan bijvoorbeeld de spatiebalk worden gebruikt om de agent te laten springen.
Wanneer je op de spatiebalk drukt, moet de agent springen.
Hiermee kun je controleren of de Rigidbody, Ground Check en springactie correct werken voordat je begint met trainen.

## 13. Config toevoegen

Voeg aan het project de yaml config bestand toe met de juiste parameters en gebruik dezelfde naam die je in je behaviour parameters hebt ingegeven.

## 14. Training starten

Wanneer de volledige omgeving werkt, kan de agent worden getraind.

Start de training met het ML-Agents trainingscommando en start daarna de Unity-scene.

Tijdens de training leert de agent op basis van de observaties en rewards wanneer hij moet springen.

## 15. Getraind model testen

Wanneer de training is afgerond, koppel je het getrainde model aan de `Behavior Parameters` van de agent.

Start vervolgens de scene.

De agent moet nu zelfstandig:

1. wachten totdat een obstakel nadert;
2. het obstakel detecteren;
3. op het juiste moment springen;
4. over het obstakel springen;
5. landen;
6. opnieuw wachten op het volgende obstakel.

Hiermee is de Jumper Agent klaar.


# Target en Zone Agent

In deze tutorial wordt uitgelegd hoe je in Unity een agent eerst een target laat aanraken en daarna naar een groene zone laat gaan.

## 1. Scene opzetten

Maak in je Unity-scene de volgende objecten aan:

* **Agent**
* **Plane**
* **Target**
* **Zone**

Plaats de agent op de plane. De target wordt op de plane geplaatst. Naast de eerste plane plaats je een tweede plane die tegen de eerste plane aanligt. Deze tweede plane wordt gebruikt als de zone.

Geef de zone een ander materiaal, bijvoorbeeld een groen materiaal, zodat deze duidelijk herkenbaar is.

### Tags

Maak de volgende tags aan:

* `Target`
* `Zone`

Geef het Target-object de tag `Target` en de zone de tag `Zone`.

## 2. Agent instellen

Selecteer de Agent en voeg de volgende componenten toe:

* `TargetZoneAgent` script
* `Decision Requester`
* `Behavior Parameters`
* `Ray Perception Sensor 3D`

Bij de `Ray Perception Sensor` worden de raycasts gebruikt om informatie uit de omgeving te verzamelen.

Stel de sensor als volgt in:

* **Max Ray Degrees**: `90`
* **End Vertical Offset**: `-1.3`

De negatieve `End Vertical Offset` zorgt ervoor dat de rays ook richting de grond kunnen kijken. Dit is belangrijk omdat de zone een platte plane is.

## 3. TargetZoneAgent script

Maak een nieuw C#-script aan met de naam `TargetZoneAgent` en koppel dit script aan de Agent.

### OnEpisodeBegin

In de `OnEpisodeBegin` methode wordt de omgeving opnieuw ingesteld wanneer een episode begint.

De target krijgt hierbij een willekeurige spawnpositie op de plane. Hierdoor staat de target tijdens iedere episode op een andere plek.

### CollectObservations

In `CollectObservations` krijgt de agent alleen informatie over de vraag of de target al gevonden is.

Dit wordt opgeslagen als een `true` of `false` waarde:

* `false` = de target is nog niet gevonden.
* `true` = de target is gevonden.

De agent kan hierdoor leren dat zijn gedrag moet veranderen nadat hij de target heeft geraakt.

### OnActionReceived

In `OnActionReceived` wordt bepaald hoe de agent kan bewegen.

De agent krijgt acties voor:

* Vooruit bewegen
* Achteruit bewegen
* Naar links draaien
* Naar rechts draaien

Daarnaast krijgt de agent een kleine negatieve reward van -0.001 wanneer hij een actie uitvoert. Hierdoor wordt de agent licht gestraft wanneer hij onnodig lang niets bereikt.

Wanneer de agent van de map valt, krijgt hij een reward van -1 en wordt de episode beëindigd.

## 4. Target aanraken

Gebruik de `OnCollisionEnter` methode om te controleren of de agent de target raakt.

Wanneer de agent de target raakt:

1. Krijgt de agent **+0.5 reward**.
2. Wordt geregistreerd dat de target is gevonden.
3. Wordt de target uitgezet, zodat de agent daarna verder kan met de tweede stap.

De agent moet hierdoor eerst leren om de target te vinden voordat hij naar de zone moet gaan.

## 5. Zone bereiken

Nadat de target is geraakt, moet de agent naar de groene zone bewegen.

Wanneer de agent daarna de zone raakt, wordt de episode beëindigd. Hierbij kan een positieve reward worden gegeven voor het succesvol bereiken van de zone.

Het belangrijkste is dat de agent de zone pas als einddoel mag gebruiken nadat de target eerst is geraakt.

## 6. Heuristic

Optioneel kan je een `Heuristic` methode toevoegen.

Hiermee kun je de agent handmatig besturen voordat je gaat trainen. Dit is handig om te controleren of de bewegingen, collision detection en het bereiken van de zone correct werken.

## 7. Config toevoegen

Voeg aan het project de yaml config bestand toe met de juiste parameters en gebruik dezelfde naam die je in je behaviour parameters hebt ingegeven.

## 8. Trainen

Wanneer de scene en het script klaar zijn, kun je de agent trainen.

Tijdens het trainen leert de agent:

1. De target te vinden.
2. De target aan te raken.
3. Daarna de groene zone te vinden.
4. De zone te bereiken zonder van de map te vallen.

Na het trainen kun je het model testen door de agent zelfstandig te laten bewegen.

