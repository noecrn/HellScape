using Photon.Pun;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LoadTileMap : MonoBehaviourPunCallbacks
{
    public Tilemap tilemap; // R�f�rence � la Tilemap dans la sc�ne
    public TileBase foundationTile; // R�f�rence � la Tile de fondation

    private void Start()
    {
        if (photonView.IsMine) // V�rifie si l'objet appartient au joueur local
        {
            // Charge le prefab de la Fondation de la TileMap depuis le dossier Resources
            GameObject foundationPrefab = Resources.Load<GameObject>("Fondations");

            // Instancie le prefab de la Fondation de la TileMap
            GameObject foundation = Instantiate(foundationPrefab);

            // Obtient la Tilemap de la Fondation
            Tilemap foundationTilemap = foundation.transform.Find("Fondation").GetComponent<Tilemap>();

            // Copie les Tiles de fondation depuis la Tilemap dans la sc�ne vers la Tilemap de la Fondation
            for (int x = foundationTilemap.cellBounds.xMin; x < foundationTilemap.cellBounds.xMax; x++)
            {
                for (int y = foundationTilemap.cellBounds.yMin; y < foundationTilemap.cellBounds.yMax; y++)
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase tile = tilemap.GetTile(tilePosition);
                    if (tile == foundationTile)
                    {
                        foundationTilemap.SetTile(tilePosition, foundationTile);
                    }
                }
            }

            // D�truit la Tilemap de la sc�ne pour �viter la duplication des Tiles
            Destroy(tilemap.gameObject);
        }
    }
}
