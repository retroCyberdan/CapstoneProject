VAR oggetti_attivabili = false

{oggetti_attivabili:
    <u>MIYUKI</u>: Devo trovare Misato, sono in pensiero per lei!
    -> END
- else:
    <u>MIYUKI</u>: Dannazione Miko, mi sono persa! Con tutta questa foschia non si vede nulla, inoltre sento dei versi raccapriccianti...
    
     * Miwa ti sta cercando
        -> risposta_prima_scelta
     * C'è qualcosa di strano in questo posto
        -> risposta_seconda_scelta
}

=== risposta_prima_scelta ===
<u>MIYUKI</u>: Miwa! Anche lei sta bene? Dopo l'incidente, mi sono risvegliata in prossimità di un tori, poi ho camminato per un pò cercando voi altre e sono giunta qui. Sai dove si trova Misato?
-> END

=== risposta_seconda_scelta ===
<u>MIYUKI</u>: Lo hai notato anche tu? Se mai dovessi sentirti stanca, riposati presso questi <color=red>santuari</color>, ti aiuteranno a stare meglio.
    # attiva_oggetto:2
~ oggetti_attivabili = true
-> END