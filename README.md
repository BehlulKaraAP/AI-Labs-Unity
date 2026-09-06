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
