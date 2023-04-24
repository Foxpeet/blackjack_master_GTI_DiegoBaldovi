using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;

    bool primeramano = true;
    public Text textoPuntosDealer;
    public Text textoPuntosPlayer;

    private void Awake()
    {
        InitCardValues();

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */
        int pos = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (j >= 10)
                {
                    values[pos] = 10;
                }
                else
                {
                    values[pos] = j + 1;
                }
                pos++;
            }
        }
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        int randpos = 0;
        System.Random random = new System.Random();
        for (int t=51; t>0; t--)
        {
            randpos = random.Next(t+1);

            Sprite cardface = faces[randpos];
            faces[randpos] = faces[t];
            faces[t] = cardface;

            int cardvalue = values[randpos];
            values[randpos] = values[t];
            values[t] = cardvalue;

        }
    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
            /*TODO:
             * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
             */
            if (dealer.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Blackjack from dealer";
                hitButton.interactable = false;
                stickButton.interactable = false;

            } else if (player.GetComponent<CardHand>().points == 21)
            {
                finalMessage.text = "Blackjack from player";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
        }
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
        string probDealerMasQueJugador = "Dealer mas puntos que jugador: " + dealerMasQueJugador() + "%";
        string probSinPasarse = "Puntos del jugador entre 17 y 21: " + sinPasarse() + "%";
        string probSePasa = "Jugador se pasa de 21 puntos: " + sePasa() + "%";
        string textProb = probDealerMasQueJugador + "\n" + "\n" + probSinPasarse + "\n" + "\n" + probSePasa;
        probMessage.text = textProb;
    }

    private string dealerMasQueJugador()
    {
        int casosFavorables = 0;
        int[] cartasEnLaMesa = new int[cardIndex];

        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int cartaDealer = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;
        int diferencia = puntuacionJugador - cartaDealer;

        //guardamos todas las cartas que hay en la mesa
        for (int i = 0; i <= cardIndex - 3; i++)
        {
            cartasEnLaMesa[i] = player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }
        cartasEnLaMesa[cardIndex - 1] = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;

        if (diferencia < 0)
        {
            //0% de probabilidad de que el dealer tenga mas puntos que el jugador
            return "0";
        }

        int valorCarta;

        //comprobamos que cartas ya han salido y afectan al calculo restandolas de la cantidad de cartas favorables
        for (int i = diferencia + 1; i <= 11; i++)
        {
            int cartasRepetidas = 0;

            if (i == cartasEnLaMesa[0])
            {
                cartasRepetidas++;
            }
            if (i == cartasEnLaMesa[1])
            {
                cartasRepetidas++;
            }
            if (i == cartasEnLaMesa[2])
            {
                cartasRepetidas++;
            }

            if (i != 10)
                casosFavorables = casosFavorables + (4 - cartasRepetidas);

            if (i == 10)
            {
                casosFavorables = casosFavorables + (16 - cartasRepetidas);
            }
        }

        //calculamos la probabilidad
        float probabilidad = (float) casosFavorables / 49;
        probabilidad = 1 - probabilidad;
        probabilidad = probabilidad * 100;
        return probabilidad.ToString();
    }

    private string sinPasarse()
    {
        int casosFavorables = 0;
        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int[] cartasEnLaMesa = new int[cardIndex];

        //guardamos las cartas que hay en la mesa
        for (int i = 0; i <= cardIndex - 3; i++)
        {
            cartasEnLaMesa[i] = player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }
        cartasEnLaMesa[cardIndex - 1] = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;

        //las cartas minimas y maximas que se pueden sacar para mantenerse entre 17 y 21
        int cartaMinima = 17 - puntuacionJugador;
        int cartaMaxima = 21 - puntuacionJugador;

        //la valor de la minima carta posible es 1
        if (cartaMinima <= 0)
        {
            cartaMinima = 1;
        }

        if (cartaMaxima <= 0)
        {
            //Hay 0% de probabilidad de sacar un numero para que la puntuacion este entre 17 y 21
            return "0";
        }
        if (cartaMinima >= 11)
        {
            //Hay 100% de probabilidad de que tu puntuacion este entre 17 y 21 con la siguiente carta
            return "100";
        }

        //recorremos todas las cartas con valores entre el minimo y maximo para mantanerse en el rango y quitamos las repetidas
        for (int i = cartaMinima; i < cartaMaxima + 1; i++)
        {
            int cartasRepetidas = 0;
            for (int j = 0; j < cartasEnLaMesa.Length; j++)
            {
                if (i == cartasEnLaMesa[j])
                {
                    cartasRepetidas++;
                }
            }
            if (i != 10)
            {
                casosFavorables += (4 - cartasRepetidas);
            }
            else
            {
                casosFavorables += (16 - cartasRepetidas);
            }
        }

        //calculamos la probabilidad
        float probabilidad = (float)casosFavorables / (52-cardIndex);
        probabilidad = probabilidad * 100;
        return probabilidad.ToString();
    }

    private string sePasa()
    {
        int casosFavorables = 0;
        int puntuacionJugador = player.GetComponent<CardHand>().points;
        int[] cartasEnLaMesa = new int[cardIndex];

        //guardamos las cartas que hay en la mesa
        for (int i = 0; i <= cardIndex - 3; i++)
        {
            cartasEnLaMesa[i] = player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value;
        }
        cartasEnLaMesa[cardIndex - 1] = dealer.GetComponent<CardHand>().cards[1].GetComponent<CardModel>().value;

        int margen = 21 - puntuacionJugador;

        if (puntuacionJugador == 21)
        {
            //100% de probabilidad de pasarte de 21 puntos
            return "100";
        }

        if (margen >= 10)
        {
            //0% de probabilidad de pasarse
            return "0";
        }

        //comprobamos las cartas que hay repetidas para no contarlas como casos favorables
        for (int i = 1; i < margen + 1; i++)
        {
            int cartasRepe = 0;

            if (i == cartasEnLaMesa[0])
            {
                cartasRepe++;
            }
            if (i == cartasEnLaMesa[1])
            {
                cartasRepe++;
            }
            if (i == cartasEnLaMesa[2])
            {
                cartasRepe++;
            }

            if (i != 10)
            {
                casosFavorables += (4 - cartasRepe);
            }
            else
            {
                casosFavorables += (16 - cartasRepe);
            }
        }

        //calculamos la probabilidad
        float probabilidad = (float)casosFavorables / (52-cardIndex);
        probabilidad = 1 - probabilidad;
        probabilidad = probabilidad * 100;
        return probabilidad.ToString();
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]);
        cardIndex++;

        //solo se calculan las probabilidades cuando la primera mano ya esta en la mesa
        if (cardIndex >= 4)
        {
            CalculateProbabilities();
        }
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        //solo se calculan las probabilidades cuando la primera mano ya esta en la mesa
        if(cardIndex>=4)
        {
            CalculateProbabilities();
        }
        textoPuntosPlayer.text = "Tus puntos: " + player.GetComponent<CardHand>().points;
    }

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        /*if (primeramano)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            primeramano = false;
        }*/

        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */
        if (player.GetComponent<CardHand>().points>21)
        {
            finalMessage.text = "Perdiste";
            hitButton.interactable = false;
            stickButton.interactable = false;

            textoPuntosDealer.text = "Puntos: " + dealer.GetComponent<CardHand>().points;
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        }
    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        if (primeramano)
        {
            dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
            primeramano = false;
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */

        if (dealer.GetComponent<CardHand>().points <= 16)
        {
            PushDealer();
            Stand();
        }
        else
        {
            int puntosdealer = dealer.GetComponent<CardHand>().points;
            int puntosplayer = player.GetComponent<CardHand>().points;
            if(puntosdealer > puntosplayer && puntosdealer<=21)
            {
                finalMessage.text = "Gana el dealer";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
            else if (puntosdealer > 21)
            {
                finalMessage.text = "Gana el jugador";
                hitButton.interactable = false;
                stickButton.interactable = false;
            } 
            else if(puntosdealer < puntosplayer)
            {
                finalMessage.text = "Gana el jugador";
                hitButton.interactable = false;
                stickButton.interactable = false;
            } else if(puntosdealer == puntosplayer)
            {
                finalMessage.text = "Empate";
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
            textoPuntosDealer.text = "Puntos: " + dealer.GetComponent<CardHand>().points;
        }

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        textoPuntosDealer.text = "Puntos:";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
}