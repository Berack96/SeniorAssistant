# SeniorAssistant
Parte del progetto SeniorAssistant che riguarda l'interazione con persone esterne.
Esso svolge un ruolo da server per raccogliere i dati provenienti dai pazienti o per riceverli.
Permette di mostrare i dati, interagire con i pazienti, con il dottore che lo segue o monitorare i dati.

## Funzionalita'
Dopo aver fatto login/essersi registrati come Dottore o Paziente si possono accedere alle seguenti funzionalità:

#### Paziente
- Scegliere il proprio dottore se non ne si ha gia' uno
- Visualizzare i propri dati anagrafici
- Visualizzare i propri dati "vitali"
- Inviare messaggi al proprio dottore
- Visualizzare i dati del dottore
- Visualizzare eventuali note che il dottore scrive

#### Dottore
- Visualizzare una lista contenente tutti i propri pazienti
- Visualizzare i dati anagrafici di ogni paziente
- Visualizzare i dati "vitali" del paziente
- Possibilita' di esportare i dati visualizzati sottofroma di file excel
- Visualizzare notifiche prodotte dal paziente nel caso i battiti superino un certo limite
- Possibilita' di regolare il valore a cui le notifiche di superamento limite vengono prodotte
- Aggiungere al paziente note (es malattie croniche, allergie ecc) o semplicemente una nota da ricordare al paziente
- Inviare messaggi al paziente
- Aggiungere un paziente al menu dei link rapidi

#### API
- L'api serve a dare l'opportunita' di inserire/modificare/leggere i dati
- Per poter utilizzare l'api e' necessario essere loggati
