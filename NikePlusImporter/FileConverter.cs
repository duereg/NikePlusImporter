using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Import.NikePlus.Entities;

namespace Import.NikePlus
{
    /// <summary>
    /// Convert NikePlus xml into a usable, strongly typed class structure
    /// </summary>
    public class FileConverter
    {
        protected string FileContents{ get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileImporter"/> class.
        /// </summary>
        /// <param name="fileContents">The file contents to import into the system.</param>
        public FileConverter(string fileContents)
        {
            if(string.IsNullOrWhiteSpace(fileContents)){
                throw new ArgumentNullException("fileContents", "The contents of the file are invalid.");
            }

            FileContents = fileContents;
        }

        /// <summary>
        /// Converts the given file contents into a <see cref="Import.NikePlus.Entities.Workout"/> instance.
        /// </summary>
        /// <returns>a <see cref="Import.NikePlus.Entities.Workout"/> instance.</returns>
        public Workout Convert()
        {
            var workFromFile = CreateWorkout(); 
            
            PopulateSnapshots(workFromFile);
            PopulateMeasurements(workFromFile);

            return workFromFile;
        }

        private void PopulateMeasurements(Workout workFromFile)
        {
            throw new NotImplementedException();
        }

        private void PopulateSnapshots(Workout workFromFile)
        {
            throw new NotImplementedException();
        }

        private Workout CreateWorkout()
        {
            var workFromFile = new Workout();
            var nikePlusDocument = new XmlDocument();
            nikePlusDocument.LoadXml(this.FileContents);
            
            workFromFile.Name = GetValue<String>(nikePlusDocument, "/sportsData/runSummary/workoutName");
            workFromFile.Comments = GetValue<String>(nikePlusDocument, "/sportsData/runSummary/comments");
            workFromFile.EventDate = GetValue<DateTime>(nikePlusDocument, "/sportsData/runSummary/time");
            workFromFile.Distance = GetValue<int>(nikePlusDocument, "/sportsData/runSummary/distance");
            workFromFile.Duration = GetValue<int>(nikePlusDocument, "/sportsData/runSummary/duration");
            workFromFile.Calories = GetValue<short>(nikePlusDocument, "/sportsData/runSummary/calories"); 

            return workFromFile;
        }

        private T GetValue<T>(XmlDocument xmlDoc, string xPath){
            T value = default(T);

            var node = xmlDoc.SelectSingleNode(xPath);

            if(node != null){ 
                var values = node.Cast<T>();
                value = values.FirstOrDefault();
            }

            return value;
        }

        
    /**
     * Imports the workout from the NikePlus document into the database
     * @return Saved workout (never null)
     * @throws NumberFormatException If a string is read when a number is expected
     * @throws DateFormatException If the format of a date is not valid
     * @throws WorkoutDAOSysException If there is an error saving the workout in the database
     */
        /*
    private Workout importWorkout() throws NumberFormatException, WorkoutDAOSysException, DateFormatException {
  
        // Interval of time (in seconds) between 2 measurements
        snapshotExtendedDataNode = nikePlusDocument.selectSingleNode("/sportsData/extendedDataList/extendedData");
        assert (snapshotExtendedDataNode != null);
        intervalValueValue = snapshotExtendedDataNode.valueOf("@intervalValue");
        assert (intervalValueValue != null);
        index = intervalValueValue.indexOf("."); // Remove everything after the "."
        if (index >= 0) {
            intervalValueValue = intervalValueValue.substring(0, index);
        }
        intervalValue = Short.parseShort(intervalValueValue);
        assert (intervalValue > 0);
        
        // Create a new workout and save the workout in the database
        try {
            workout = new Workout(workoutId, workoutImportedOn, workoutName, workoutDate, workoutDistance, workoutDuration, workoutCalories, workoutComments, intervalValue);
            workoutDAO.saveWorkout(workout);
        } catch (IllegalArgumentException e) {
            // This should never happen
            log.error("The Workout to create is not valid", e);
            throw new AssertionError("This should never happen - method importWorkout() from class 'NikePlus2': " + e.getMessage());
        }
        
        assert (workout != null);
        return workout;
    }

        */
    }
}


 
//import org.runningtracker.engine.db.WorkoutDAO;
//import org.runningtracker.engine.exceptions.WorkoutDAOSysException;
//import java.util.List;
//import org.apache.log4j.Logger;
//import org.dom4j.Document;
//import org.dom4j.Node;
//import org.joda.time.DateTime;
//import org.runningtracker.engine.entities.Measurement;
//import org.runningtracker.engine.entities.Snapshot;
//import org.runningtracker.engine.entities.Workout;
//import org.runningtracker.engine.exceptions.DateFormatException;

/////
// * <UL>
// * <LI>Permits to import NikePlus files (version 2) into the embedded database</LI>
// * <LI>The method <CODE>importFile()</CODE> imports the XML document into the embedded database</LI>
// * </UL>
// * @author Francois Duchemin
// */
//public class NikePlus2 implements Importer {
    
//    /** DOM document (supports XPath), which contains the NikePlus XML */
//    private Document nikePlusDocument;
    
//    /** Comments to give to the imported NikePlus workout */
//    private String workoutComments;
    
//    /** Workout DAO for interaction with the database */
//    private WorkoutDAO workoutDAO;
    
//    /** Logger */
//    private static final Logger log = Logger.getLogger(NikePlus2.class.getName());

    
//    /**
//     * Imports the snapshots (kmSplit, mileSplit, userClick) from the NikePlus document into the database
//     * @param m_workout Workout that contains the snapshots (cannot be null)
//     * @throws NumberFormatException If a string is read when a number is expected
//     * @throws WorkoutDAOSysException If there is an error saving a snapshot in the database
//     */
//    private void importSnapshots(Workout m_workout) throws NumberFormatException, WorkoutDAOSysException {
//        Node snapshotListNode; // <snapShotList> node (kmSplit, mileSplit, userClick)
//        List listOfSnapshotsList; // List of <snapShotList>
//        List listOfSnapshots; // List of <snapShot>
//        int index;
        
//        Node snapshotNode; // <snapShot> node
//        Snapshot snapshot; // Snapshot to create
//        long snapshotId = 0;
        
//        String snapshotTypeValue; // kmSplit, mileSplit, userClick
//        SnapshotType snapshotType;
        
//        String snapshotEventValue; // pause, resume, powerSong...
//        SnapshotEvent snapshotEvent;
        
//        Node snapshotDistanceNode; // Distance covered since the beginning of the workout
//        String snapshotDistanceValue;
//        int snapshotDistance;
        
//        Node snapshotDurationNode; // Duration since the beginning of the workout
//        String snapshotDurationValue;
//        int snapshotDuration;
        
//        Node snapshotPaceNode;
//        String snapshotPaceValue;
//        int snapshotPace;
        
//        // -- Import the snapshots --
//        assert (m_workout != null);
        
//        listOfSnapshotsList = nikePlusDocument.selectNodes("/sportsData/snapShotList"); // List of <snapShotList snapShotType="xxxxx">
//        assert (listOfSnapshotsList != null && listOfSnapshotsList.size() >= 0);
//        for (Object snapshotListObject : listOfSnapshotsList) {
//            // Get the current snapshotlist node
//            snapshotListNode = (Node) snapshotListObject; // halfMarathonSplit, kmSplit, mileSplit or userClick snapshotList
//            assert (snapshotListNode != null);
//            snapshotTypeValue = snapshotListNode.valueOf("@snapShotType"); // "halfMarathonSplit", "kmSplit", "mileSplit", "userClick"
//            assert (snapshotTypeValue != null);
//            snapshotType = SnapshotType.valueOf(snapshotTypeValue);
//            assert (snapshotType != null);
            
//            // Get the snapshots of the snapshotList
//            listOfSnapshots = snapshotListNode.selectNodes("snapShot");
//            assert (listOfSnapshots != null && listOfSnapshots.size() >= 0);
//            for (Object snapshotObject : listOfSnapshots) {
//                // Get the current snapshot
//                snapshotNode = (Node) snapshotObject;
//                assert (snapshotNode != null);
                
//                // Snapshot event
//                snapshotEventValue = snapshotNode.valueOf("@event");
//                assert (snapshotEventValue != null);
//                if (snapshotEventValue.length() > 0) {
//                    snapshotEvent = SnapshotEvent.valueOf(snapshotEventValue);
//                } else {
//                    snapshotEvent = SnapshotEvent.noEvent;
//                }
//                assert (snapshotEvent != null);
                
//                // Snapshot's duration (in milliseconds) (since the beginning of the workout)
//                snapshotDurationNode = snapshotNode.selectSingleNode("duration");
//                assert (snapshotDurationNode != null);
//                snapshotDurationValue = snapshotDurationNode.getStringValue();
//                assert (snapshotDurationValue != null);
//                index = snapshotDurationValue.indexOf("."); // Remove everything after the "."
//                if (index >= 0) {
//                    snapshotDurationValue = snapshotDurationValue.substring(0, index);
//                }
//                snapshotDuration = Integer.parseInt(snapshotDurationValue);
//                assert (snapshotDuration >= 0);
                
//                // Snapshot's distance (in decimeters) (since the beginning of the workout)
//                snapshotDistanceNode = snapshotNode.selectSingleNode("distance");
//                assert (snapshotDistanceNode != null);
//                snapshotDistanceValue = snapshotDistanceNode.getStringValue();
//                assert (snapshotDistanceValue != null);
//                snapshotDistance = Distance.getDecimeterDistance(snapshotDistanceValue);
//                assert (snapshotDistance >= 0);
                
//                // Snapshot's pace (in milliseconds)
//                snapshotPaceNode = snapshotNode.selectSingleNode("pace");
//                if (snapshotPaceNode != null) {
//                    snapshotPaceValue = snapshotPaceNode.getStringValue();
//                    assert (snapshotPaceValue != null);
//                    index = snapshotPaceValue.indexOf("."); // Remove everything after the "."
//                    if (index >= 0) {
//                        snapshotPaceValue = snapshotPaceValue.substring(0, index);
//                    }
//                    snapshotPace = Integer.parseInt(snapshotPaceValue);
//                } else {
//                    snapshotPace = 0;
//                }
//                assert (snapshotPace >= 0);
                
//                // Create a new snapshot and try to save the snapshot in the database
//                try {
//                    snapshot = new Snapshot(snapshotId, m_workout, snapshotType, snapshotEvent, snapshotDistance, snapshotDuration, snapshotPace);
//                    workoutDAO.saveSnapshot(snapshot);
//                } catch (IllegalArgumentException e) {
//                    // This should never happen
//                    log.error("The Snapshot to create is not valid", e);
//                    throw new AssertionError("This should never happen - method importSnapshot() from class 'NikePlus2': " + e.getMessage());
//                }
//            }
//        }
//    }
    
//    /**
//     * Imports the measurements from the NikePlus document into the database<BR>
//     * Measurements are taken every 10 seconds<BR>
//     * The measurements are in kilometer
//     * @param m_workout Workout that contains the measurements (cannot be null)
//     * @throws NumberFormatException If a string is read when a number is expected
//     * @throws WorkoutDAOSysException If there is an error saving a measurement in the database
//     */
//    private void importMeasurements(Workout m_workout) throws NumberFormatException, WorkoutDAOSysException {
//        Node extendedDataNode; // <extendedData dataType="distance" intervalType="time" intervalUnit="s" intervalValue="10">
//        String extendedDataValue; // content of the <extendedData>
//        String intervalValueValue; // Interval of time (in seconds) between 2 measurements
//        short intervalValue;
//        String[] extendedDataValueSplit; // table containing the measurements
//        Measurement measurement;
//        int measurementDistance; // Distance covered during the measurement (in decimeters)
//        int measurementDuration; // Duration since the beginning of the workout (in milliseconds)
//        long measurementId = 0;
//        int index;
        
//        // -- Import the measurements --
//        assert (m_workout != null);
        
//        extendedDataNode = nikePlusDocument.selectSingleNode("/sportsData/extendedDataList/extendedData");
//        assert (extendedDataNode != null);
//        assert (extendedDataNode.valueOf("@dataType").compareTo("distance") == 0);
//        assert (extendedDataNode.valueOf("@intervalType").compareTo("time") == 0);
//        assert (extendedDataNode.valueOf("@intervalUnit").compareTo("s") == 0);
        
//        intervalValueValue = extendedDataNode.valueOf("@intervalValue");
//        assert (intervalValueValue != null);
//        index = intervalValueValue.indexOf("."); // Remove everything after the "."
//        if (index >= 0) {
//            intervalValueValue = intervalValueValue.substring(0, index);
//        }
//        intervalValue = Short.parseShort(intervalValueValue);
//        assert (intervalValue > 0);
        
//        extendedDataValue = extendedDataNode.getStringValue();
//        assert (extendedDataValue != null);
//        extendedDataValueSplit = extendedDataValue.split(", ?");
//        assert (extendedDataValueSplit.length > 0);
        
//        // Go through the list of extended data
//        for (int i = 1; i < extendedDataValueSplit.length; i++) {
//            // Distance covered during the measurement (snapshotExtendedDataValueSplit[0] is always 0.0)
//            measurementDistance = Distance.getDecimeterDistance(extendedDataValueSplit[i]) - Distance.getDecimeterDistance(extendedDataValueSplit[i-1]);
//            assert (measurementDistance >= 0);
            
//            measurementDuration = i * intervalValue * 1000; // Duration since the beginning of the workout (in milliseconds)
            
//            // Create a new measurement and save the measurement in the database
//            try {
//                measurement = new Measurement(measurementId, m_workout, measurementDistance, measurementDuration);
//                workoutDAO.saveMeasurement(measurement);
//            } catch (IllegalArgumentException e) {
//                // This should never happen
//                log.error("The Measurement to create is not valid", e);
//                throw new AssertionError("This should never happen - method importMeasurement() from class 'NikePlus2': " + e.getMessage());
//            }
//        }
//    }
    
//    /**
//     * Imports the NikePlus document into the database<BR>
//     * <UL>
//     * <LI>Import the workout from the document</LI>
//     * <LI>Import the snapshots from the document</LI>
//     * <LI>Import the measurements from the document</LI>
//     * </UL>
//     * @return Imported workout (never null)
//     * @throws NumberFormatException If a string is read when a number is expected
//     * @throws DateFormatException If the format of a date is invalid
//     * @throws WorkoutDAOSysException If there is a DB error
//     */
//    @Override
//    public Workout importNikePlusDocument() throws NumberFormatException, DateFormatException, WorkoutDAOSysException {
//        Workout workout;                // Saved workout
//        long startImportingDocumentTime;// Time when the import of the file starts
//        long endImportingDocumentTime;  // Time when the import of the file ends
//        long loadingTime;               // Time to load the NikePlus document
        
//        startImportingDocumentTime = System.currentTimeMillis();

//        // Import the content of the NikePlus document into the embedded database
//        workout = importWorkout();
//        assert (workout != null);
//        importSnapshots(workout);
//        importMeasurements(workout);

//        endImportingDocumentTime = System.currentTimeMillis();
//        loadingTime = endImportingDocumentTime - startImportingDocumentTime;
        
//        log.info("Import results: the NikePlus document has been imported in " + loadingTime + " ms");
        
//        assert (workout != null);
//        return workout;
//    }
//}