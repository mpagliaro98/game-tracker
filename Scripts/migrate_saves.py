import os
import json
import argparse


MIGRATED_FOLDER = "migrated"

MIN_SCORE = 0
CATEGORIES = []


def migrate_games(path):
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "games.json")
    result_file_path = os.path.join(result_path, "modelobjects.json")
    result_json = []
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_game = {}
            new_game["TypeName"] = "GameObject"
            for key in game:
                if key == "referenceKey":
                    new_game["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_game["Name"] = game[key]
                elif key == "comment":
                    new_game["Comment"] = game[key]
                elif key == "finalScoreManual":
                    new_game["ManualScore"] = game[key]
                elif key == "status":
                    new_game["Status"] = game[key]["objectKey"]
                elif key == "categoryValues":
                    cat_vals = []
                    for category_value in game[key]:
                        new_cat_val = {}
                        new_cat_val["TypeName"] = "CategoryValue"
                        for cat_val_key in category_value:
                            if cat_val_key == "ratingCategory":
                                new_cat_val["Category"] = category_value[cat_val_key]["objectKey"]
                            elif cat_val_key == "pointValue":
                                new_cat_val["PointValue"] = category_value[cat_val_key]
                        cat_vals.append(new_cat_val)
                    if len(cat_vals) > 0:
                        new_game["CategoryValues"] = cat_vals
                    else:
                        cat_vals = []
                        for cat in CATEGORIES:
                            new_cat_val = {}
                            new_cat_val["TypeName"] = "CategoryValue"
                            new_cat_val["Category"] = cat["UniqueID"]
                            new_cat_val["PointValue"] = MIN_SCORE
                            cat_vals.append(new_cat_val)
                        new_game["CategoryValues"] = cat_vals
                elif key == "ignoreCategories":
                    new_game["IgnoreCategories"] = game[key]
                elif key == "platform":
                    new_game["Platform"] = game[key]["objectKey"]
                elif key == "platformPlayedOn":
                    new_game["PlatformPlayedOn"] = game[key]["objectKey"]
                elif key == "completionCriteria":
                    new_game["CompletionCriteria"] = game[key]
                elif key == "completionComment":
                    new_game["CompletionComment"] = game[key]
                elif key == "timeSpent":
                    new_game["TimeSpent"] = game[key]
                elif key == "releaseDate":
                    new_game["ReleaseDate"] = game[key]
                elif key == "acquiredOn":
                    new_game["AcquiredOn"] = game[key]
                elif key == "startedOn":
                    new_game["StartedOn"] = game[key]
                elif key == "finishedOn":
                    new_game["FinishedOn"] = game[key]
                elif key == "isRemaster":
                    new_game["IsRemaster"] = game[key]
                elif key == "originalGame":
                    new_game["OriginalGame"] = game[key]["objectKey"]
                elif key == "useOriginalGameScore":
                    new_game["UseOriginalGameScore"] = game[key]
                elif key == "compilation":
                    new_game["Compilation"] = game[key]["objectKey"]
            result_json.append(new_game)
    
    file_path = os.path.join(path, "compilations.json")
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_game = {}
            new_game["TypeName"] = "GameCompilation"
            for key in game:
                if key == "referenceKey":
                    new_game["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_game["Name"] = game[key]
                elif key == "comment":
                    new_game["Comment"] = game[key]
                elif key == "finalScoreManual":
                    new_game["ManualScore"] = game[key]
                elif key == "status":
                    new_game["Status"] = game[key]["objectKey"]
                elif key == "ignoreCategories":
                    new_game["IgnoreCategories"] = game[key]
                elif key == "platform":
                    new_game["Platform"] = game[key]["objectKey"]
                elif key == "platformPlayedOn":
                    new_game["PlatformPlayedOn"] = game[key]["objectKey"]
                elif key == "completionCriteria":
                    new_game["CompletionCriteria"] = game[key]
                elif key == "completionComment":
                    new_game["CompletionComment"] = game[key]
                elif key == "timeSpent":
                    new_game["TimeSpent"] = game[key]
                elif key == "releaseDate":
                    new_game["ReleaseDate"] = game[key]
                elif key == "acquiredOn":
                    new_game["AcquiredOn"] = game[key]
                elif key == "startedOn":
                    new_game["StartedOn"] = game[key]
                elif key == "finishedOn":
                    new_game["FinishedOn"] = game[key]
                elif key == "isRemaster":
                    new_game["IsRemaster"] = game[key]
                elif key == "originalGame":
                    new_game["OriginalGame"] = game[key]["objectKey"]
                elif key == "useOriginalGameScore":
                    new_game["UseOriginalGameScore"] = game[key]
                elif key == "compilation":
                    new_game["Compilation"] = game[key]["objectKey"]
            
            cat_vals = []
            for cat in CATEGORIES:
                new_cat_val = {}
                new_cat_val["TypeName"] = "CategoryValue"
                new_cat_val["Category"] = cat["UniqueID"]
                new_cat_val["PointValue"] = MIN_SCORE
                cat_vals.append(new_cat_val)
            new_game["CategoryValues"] = cat_vals
            result_json.append(new_game)
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def migrate_statuses(path):
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "completion_statuses.json")
    result_file_path = os.path.join(result_path, "statuses.json")
    result_json = []
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_game = {}
            new_game["TypeName"] = "StatusGame"
            for key in game:
                if key == "referenceKey":
                    new_game["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_game["Name"] = game[key]
                elif key == "color":
                    new_game["Color"] = game[key]
                elif key == "useAsFinished":
                    new_game["UseAsFinished"] = game[key]
                elif key == "excludeFromStats":
                    new_game["ExcludeFromStats"] = game[key]
            result_json.append(new_game)
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def migrate_platforms(path):
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "platforms.json")
    result_file_path = os.path.join(result_path, "platforms.json")
    result_json = []
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_game = {}
            new_game["TypeName"] = "Platform"
            for key in game:
                if key == "referenceKey":
                    new_game["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_game["Name"] = game[key]
                elif key == "color":
                    new_game["Color"] = game[key]
                elif key == "releaseYear":
                    new_game["ReleaseYear"] = game[key]
                elif key == "acquiredYear":
                    new_game["AcquiredYear"] = game[key]
                elif key == "abbreviation":
                    new_game["Abbreviation"] = game[key]
            result_json.append(new_game)
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def migrate_categories(path):
    global CATEGORIES
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "rating_categories.json")
    result_file_path = os.path.join(result_path, "ratingcategories.json")
    result_json = []
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_cat = {}
            new_cat["TypeName"] = "RatingCategoryWeighted"
            for key in game:
                if key == "referenceKey":
                    new_cat["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_cat["Name"] = game[key]
                elif key == "comment":
                    new_cat["Comment"] = game[key]
                elif key == "weight":
                    new_cat["Weight"] = game[key]
            result_json.append(new_cat)
            CATEGORIES.append(new_cat)
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def migrate_ranges(path):
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "score_ranges.json")
    result_file_path = os.path.join(result_path, "scoreranges.json")
    result_json = []
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_array = json.loads(f.read())
        f.close()
        for game in json_array:
            new_game = {}
            new_game["TypeName"] = "ScoreRange"
            for key in game:
                if key == "referenceKey":
                    new_game["UniqueID"] = game[key]["objectKey"]
                elif key == "name":
                    new_game["Name"] = game[key]
                elif key == "color":
                    new_game["Color"] = game[key]
                elif key == "scoreRelationship":
                    if game[key]["objectKey"] == "11111111-1111-1111-1111-111111111111":
                        new_game["ScoreRelationship"] = "00000001-0000-0000-0000-000000000000"
                    elif game[key]["objectKey"] == "22222222-2222-2222-2222-222222222222":
                        new_game["ScoreRelationship"] = "00000002-0000-0000-0000-000000000000"
                    elif game[key]["objectKey"] == "33333333-3333-3333-3333-333333333333":
                        new_game["ScoreRelationship"] = "00000003-0000-0000-0000-000000000000"
                elif key == "valueList":
                    new_game["ValueList"] = game[key]
            result_json.append(new_game)
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def migrate_settings(path):
    global MIN_SCORE
    result_path = os.path.join(path, MIGRATED_FOLDER)
    if not os.path.exists(result_path):
        os.makedirs(result_path)
    
    file_path = os.path.join(path, "settings.json")
    result_file_path = os.path.join(result_path, "settings.json")
    result_json = {}
    result_json["TypeName"] = "SettingsGame"
    if os.path.exists(file_path):
        f = open(file_path, "r")
        json_object = json.loads(f.read())
        f.close()
        for key in json_object:
            if key == "minScore":
                result_json["MinScore"] = json_object[key]
                MIN_SCORE = json_object[key]
            elif key == "maxScore":
                result_json["MaxScore"] = json_object[key]
    
    file_content = json.dumps(result_json)
    f = open(result_file_path, "w")
    f.write(file_content)
    f.close()


def parse_args():
    parser = argparse.ArgumentParser(description="Game tracker save migration")
    parser.add_argument("path", help="Folder containing un-migrated saves")
    args = parser.parse_args()
    return args


def main():
    args = parse_args()
    migrate_settings(args.path)
    migrate_categories(args.path)
    migrate_games(args.path)
    migrate_statuses(args.path)
    migrate_platforms(args.path)
    migrate_ranges(args.path)


if __name__ == '__main__':
    main()
