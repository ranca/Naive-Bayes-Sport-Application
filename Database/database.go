package main

import (
	_ "github.com/denisenkom/go-mssqldb"
	"database/sql"
	"context"
	"log"
	"fmt"
	"io/ioutil"
	"net/http"
	"encoding/json"
)

type Person struct {
	Id				int			`json:"id"`
	Name			string		`json:"fullName"`
}

type Player struct {
	Person			Person		`json:"person"`
}

type RosterResponse struct {
	Players			[]Player	`json:"roster"`
}

type Team struct {
	Id				int		`json:"id"`
	Name			string	`json:"name"`
	RosterResponse	RosterResponse	`json:"roster"`
}

type TeamResponse struct {
	Teams			[]Team 		`json:"teams"`
}

type FullPerson struct {
	Id				    int		`json:"id"`
    Name			    string	`json:"fullName"`
    BirthCity           string      `json:"birthCity"`
    BirthStateProvince  string          `json:"birthStateProvince"`
    BirthCountry        string          `json:"birthCountry"`
    Nationality         string          `json:"nationality"`
}

type PlayerResponse struct {
    Players              []FullPerson    `json:"people"`
}

func ReadPlayers() (int, error) {

    rows, err := ExecuteQueryReader("SELECT Id, Name FROM dbo.Players p;")
    if err != nil {
        return -1, err
    }

    defer rows.Close()

    var count int

    // Iterate through the result set.
    for rows.Next() {
        var name string
        var id int

        // Get values from row.
        err := rows.Scan(&id, &name)
        if err != nil {
            return -1, err
        }

        fmt.Printf("ID: %d, Name: %s\n", id, name)
        count++
    }

    return count, nil
}

func PopulateTeams() (int, error) {

	count := 0

	for id := 1; id < 25; id++ {
		resp, err := http.Get(fmt.Sprintf("https://statsapi.web.nhl.com/api/v1/teams/%d?expand=team.roster", id))
		if err != nil {
            fmt.Printf("Error getting a team from an api!\n")
			log.Fatal(err)
		}
		body, readErr := ioutil.ReadAll(resp.Body)
		if readErr != nil {
			log.Fatal(readErr)
			continue
		}
		bytes := []byte(string(body))
			// Fill the record with the data from the JSON
		var record TeamResponse

		// Use json.Decode for reading streams of JSON data
		json.Unmarshal(bytes, &record)

        //InsertTeam(record.Teams[0].Id,record.Teams[0].Name)
		
		for _, element := range record.Teams[0].RosterResponse.Players {
            p, err := GetPlayer(element.Person.Id)
            if err != nil {
                fmt.Printf("Error fetching a player")
            }
            UpdatePlayer(p)
		}
	}
	return count, nil
}

func InsertTeam(id int, name string) (int64, error) {
	ctx := context.Background()
    var err error

    if db == nil {
        return -1, nil
    }

    // Check if database is alive.
    err = db.PingContext(ctx)
    if err != nil {
        return -1, err
    }

    tsql := "INSERT INTO dbo.Teams (Id, Name) VALUES (@Id, @Name);"

    stmt, err := db.Prepare(tsql)
    if err != nil {
       return -1, err
    }
    defer stmt.Close()

    stmt.QueryRowContext(
        ctx,
        sql.Named("Id", id),
		sql.Named("Name", name))

    return 1, nil
}

func InsertPlayer(id int, name string) (int64, error) {
	ctx := context.Background()
    var err error

    if db == nil {
        return -1, nil
    }

    // Check if database is alive.
    err = db.PingContext(ctx)
    if err != nil {
        return -1, err
    }

    tsql := "INSERT INTO dbo.Players (Id, Name) VALUES (@Id, @Name);"

    stmt, err := db.Prepare(tsql)
    if err != nil {
       return -1, err
    }
    defer stmt.Close()

    stmt.QueryRowContext(
        ctx,
        sql.Named("Id", id),
		sql.Named("Name", name))

    return 1, nil
}

func UpdatePlayerTeams(id int, teamId int) (int64, error) {
    cnt, err := ExecuteNonQuery("UPDATE dbo.Players SET TeamId = @TeamId WHERE Id = @Id;",
	sql.Named("Id", id),
    sql.Named("TeamId", teamId))
    if err != nil {
        fmt.Printf("Update failed")
    }
    return cnt, nil
}

func UpdatePlayer(player FullPerson) (int64, error) {
    var birthPlace string
    if birthPlace = fmt.Sprintf("%s, %s", player.BirthCity, player.BirthCountry); player.BirthStateProvince != "" {
        birthPlace = fmt.Sprintf("%s, %s, %s", player.BirthCity, player.BirthStateProvince, player.BirthCountry)
    }
    fmt.Printf("Updating %s\n", player.Name)
    ExecuteNonQuery(`UPDATE dbo.Players 
                        SET Name = @Name,
                            BirthPlace = @BirthPlace,
                            Nationality = @Nationality
                        WHERE Id = @Id;`,
                    sql.Named("Id", player.Id),
                    sql.Named("Name", player.Name),
                    sql.Named("BirthPlace", birthPlace),
                    sql.Named("Nationality", player.Nationality))

    return 1, nil
}

func UpdatePlayers() (int, error) {

    rows, err := ExecuteQueryReader("SELECT Id, Name, TeamId FROM dbo.Players p;")
    if err != nil {
        return -1, err
    }

    // executed after return
    defer rows.Close()

    var count int

    // Iterate through the result set.
    for rows.Next() {
        var name string
        var id int

        // Get values from row.
        err := rows.Scan(&id, &name)
        if err != nil {
            return -1, err
        }

        player, err := GetPlayer(id)
        if err != nil {
            return -1, err
        }

        UpdatePlayer(player)

        fmt.Printf("Updating... ID: %d, Name: %s\n", id, name)
		count++
    }

    return count, nil
}

func ExecuteQueryReader(query string) (*sql.Rows, error) {
    ctx := context.Background()
    // Check if database is alive.
    err := db.PingContext(ctx)
    if err != nil {
        return nil, err
    }

    // Execute query
    rows, err := db.QueryContext(ctx, query)
    if err != nil {
        return nil, err
    }
    
	return rows, err
}

func ExecuteNonQuery(query string, args ...interface{}) (int64, error) {
	ctx := context.Background()
    var err error

    if db == nil {
        return -1, nil
    }

    // Check if database is alive.
    err = db.PingContext(ctx)
    if err != nil {
        return -1, err
    }

    result, err := db.ExecContext(
        ctx,
        query,
        args...)
    if err != nil {
        log.Fatal("Error updating Player: ", err.Error())
        return -1, err
    }
    
    return result.RowsAffected()
}

func GetPlayer(id int) (FullPerson, error) {
	resp, err := http.Get(fmt.Sprintf("https://statsapi.web.nhl.com/api/v1/people/%d", id))
		if err != nil {
			log.Fatal(err)
		}
		body, readErr := ioutil.ReadAll(resp.Body)
		if readErr != nil {
			log.Fatal(readErr)
		}
        bytes := []byte(string(body))
        
		var record PlayerResponse

        json.Unmarshal(bytes, &record)
        
        return record.Players[0], nil
}