<?php
header("Content-Type: application/json");

$host = 'localhost';
$db = 'hr';
$user = 'root';
$pass = '';
$charset = 'utf8mb4';

$dsn = "mysql:host=$host;dbname=$db;charset=$charset";
$options = [
    PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
    PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC,
    PDO::ATTR_EMULATE_PREPARES => false,
];

$pdo = new PDO($dsn, $user, $pass, $options);

$output = [];
try {
    if ($_SERVER['REQUEST_METHOD'] === 'GET') {
        if (isset($_GET['action']) && $_GET['action'] === 'get_employees') {
            $sql = "SELECT e.id, e.name, e.salary, e.bdate, d.dname AS department 
                     FROM employee AS e 
                     INNER JOIN department AS d ON e.dno = d.dnumber";
            $stmt = $pdo->prepare($sql);
            $stmt->execute();
            $output = $stmt->fetchAll();
        } else {
            $stmt = $pdo->query("SELECT userid, username, pass, email FROM accounts");
            $output = $stmt->fetchAll();
        }
    } elseif ($_SERVER['REQUEST_METHOD'] === 'POST') {
        $input = json_decode(file_get_contents('php://input'), true);
        if (isset($input['action']) && $input['action'] === 'add_employee') {
            // Validate and sanitize employee data (optional)
            if (!isset($input['name']) || !isset($input['dno']) || !isset($input['salary'])) {
                throw new Exception("Missing required employee data (name, department number, salary)");
            }
            
            $sql = "INSERT INTO employee (name, dno, salary) VALUES (?, ?, ?)";
            $stmt = $pdo->prepare($sql);
            $stmt->execute([$input['name'], $input['dno'], $input['salary']]);
            $output = ['message' => 'Employee added successfully'];

        } else {
            $sql = "INSERT INTO accounts (username, pass, email) VALUES (?, ?, ?)";
            $stmt = $pdo->prepare($sql);
            $stmt->execute([$input['username'], $input['pass'], $input['email']]);
            $output = ['message' => 'User added successfully'];
        }
    }
    echo json_encode($output);
} catch (Exception $e) {
    echo json_encode(['error' => $e->getMessage()]);
}
?>
