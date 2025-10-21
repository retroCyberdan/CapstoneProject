VAR oggetti_ricevuti = false

{oggetti_ricevuti:
    <u>MIWA</u>: Spero che quelle due si facciano vive il prima possibile...
    -> END
- else:
    <u>MIWA</u>: Ah finalmente... Almeno tu ti sei fatta viva! Vorrei capire dove diamine si sono cacciate Misato e Miyuki...
    
     * Vorrei tanto saperlo anche io...
        -> risposta_prima_scelta
     * Sono sicura che stanno bene.
        -> risposta_seconda_scelta
}

=== risposta_prima_scelta ===
<u>MIWA</u>: Fanno sempre così! Spriscono per ore prima di farsi rivedere. Spero non gli sia successo nulla, questo posto non mi piace...
-> END

=== risposta_seconda_scelta ===
<u>MIWA</u>: Forse hai ragione. Hai notato anche tu questa insolita foschia? Tieni, ho trovato questa nell'autobus, magari quelle due ti vedranno! # attiva_oggetto:0 # attiva_oggetto:1
~ oggetti_ricevuti = true
-> END