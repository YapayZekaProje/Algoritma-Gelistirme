
// Greedy Search Fath Finding for map 1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedySearch : MonoBehaviour
{
    Grid grid; // Önceden yapılmış Grid sınıfına ait bir nesne
    Player player; // Önceden yapılmış Player sınıfına ait bir nesne
    public Transform seeker, target; // Hedefe ulaşmaya çalışan ve hedef olan Transform bileşenleri (Bunlar haritada object olarak yapılmış)
    public bool driveable = true; // Oyuncunun hareket edebilir durumda olup olmadığını kontrol eden 

    private void Awake()
    {
        grid = GetComponent<Grid>(); // Bu objedeki Grid bileşenini al
        player = FindObjectOfType<Player>();  // Haritadaki Player objesini bul
    }

    private void Update()
    {
        FindPath(seeker.position, target.position); // Seeker ve hedef arasındaki yolu bul
        GoToTarget(); // Hedefe gitmek için gerekli işlemleri yap
    }

    void GoToTarget()
    {
        // Eğer bir yol varsa ve oyuncu hareket edebiliyorsa
        if (grid.path1 != null && grid.path1.Count > 0 && driveable) // path1 grid sınıfında List<Node> olarak tanımlanmış (yol temsil ediyor)
        {
            Vector3 hedefNokta = grid.path1[0].WorldPosition;  // Yolun ilk düğümünün konumunu al  
            player.LookToTarget(hedefNokta); // Oyuncuyu hedefe doğru döndür
            player.GidilcekYer(hedefNokta);  // Hedef pozisyonunu oyuncuya ilet
            //("hedefNokta" player sınıfında "GidilcekYer" fonkisyonun parametresi olarak tanımlanmış)
        }
    }

    void FindPath(Vector3 startPoz, Vector3 targetPoz)
    {
        // startNode ve targetNode nodleri oluşturuyoruz
        Node startNode = grid.NodeFromWorldPoint(startPoz); // Başlangıç pozisyonundan node'u bul
        Node targetNode = grid.NodeFromWorldPoint(targetPoz); // Hedef pozisyonundan node'u bul

        List<Node> openSet = new List<Node>(); // Kontrol edilecek düğümleri içeren liste
        List<Node> closedSet = new List<Node>(); // Zaten kontrol edilmiş düğümleri içeren liste

        openSet.Add(startNode); // Başlangıç düğümünü açık listeye ekle

        while (openSet.Count > 0) // Açık listede düğüm olduğu sürece döngü devam eder
        {
            Node currentNode = openSet[0]; // İlk düğümü al

            // En düşük hCost değerine sahip düğümü bul
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].hCost < currentNode.hCost) // "hCost" Node sınıfında tanımlanmış
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode); // Şu anki düğümü açık listeden çıkar
            closedSet.Add(currentNode); // Şu anki düğümü kapalı listeye ekle

            // Eğer hedefe ulaşıldıysa yolu geri izleyerek oluştur
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode); // Yolu oluştur
                return;
            }

            // Şu anki düğümün tüm komşularını kontrol et
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                // Eğer düğüm geçilebilir değilse veya zaten kapalı listede ise devam et
                if (!neighbour.Walkable || closedSet.Contains(neighbour)) // "Walkable" Node sınıfında tanımlanmış
                {
                    continue;
                }

                // Komşu düğüm için heuristic maliyetini GetDistance fonk. kullanarak hesapla
                neighbour.hCost = GetDistance(neighbour, targetNode); 

                // Eğer komşu düğüm açık listede değilse, ekle
                if (!openSet.Contains(neighbour))
                {
                    neighbour.parent = currentNode;  // Yolun geri izlenebilmesi için ebeveyni ata ("parent" Node sınıfında tanımlanmış)
                    openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>(); // Oluşturulan yolu saklamak için bir liste
        Node currentNode = endNode; // Yolu hedef düğümden başlat

        while (currentNode != startNode) // Başlangıç düğümüne ulaşana kadar geri izle
        {
            path.Add(currentNode); // Şu anki düğümü yola ekle
            currentNode = currentNode.parent; // Bir önceki düğüme geç
        }

        path.Reverse(); // Listeyi ters çevir (başlangıçtan hedefe doğru)
        grid.path1 = path;  // Grid'deki path1 değişkenine oluşturulan yolu ata
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX); // X koordinatlarındaki farkı al
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY); // Y koordinatlarındaki farkı al

        return 10 * (dstX + dstY);  // Manhattan mesafesini hesapla (grid tabanlı yollar için uygun)
    }
}


