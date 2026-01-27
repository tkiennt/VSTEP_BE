/* ===========================
   VSTEP WRITING SYSTEM (3NF)
   Database: vstep_writing
   MySQL - utf8mb4
   =========================== */

CREATE DATABASE IF NOT EXISTS vstep_writing
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE vstep_writing;

/* ===== LOOKUP TABLES ===== */

CREATE TABLE levels (
    level_id INT AUTO_INCREMENT PRIMARY KEY,
    level_code VARCHAR(5) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

CREATE TABLE part_types (
    part_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(20) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

INSERT INTO part_types (code, description) VALUES
('writing_part1', 'Writing Part 1'),
('writing_part2', 'Writing Part 2');

CREATE TABLE practice_modes (
    mode_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

INSERT INTO practice_modes (code, description) VALUES
('full_test', 'Full test'),
('by_part', 'By part'),
('timed', 'Timed practice');

CREATE TABLE hint_types (
    hint_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

CREATE TABLE prompt_purposes (
    purpose_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

CREATE TABLE sample_types (
    sample_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(20) UNIQUE NOT NULL,
    description VARCHAR(100)
) ENGINE=InnoDB;

/* Default level for users.target_level_id FK */
INSERT INTO levels (level_code, description) VALUES ('B1', 'Default level');

/* ===== USER & PRACTICE (users table includes auth columns for API) ===== */

CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    target_level_id INT NOT NULL DEFAULT 1,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(500) NOT NULL,
    role INT NOT NULL DEFAULT 0,
    updated_at DATETIME NULL,
    is_active TINYINT(1) NOT NULL DEFAULT 1,
    FOREIGN KEY (target_level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB;

CREATE TABLE password_reset_tokens (
    token_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    token VARCHAR(500) NOT NULL,
    expires_at DATETIME NOT NULL,
    used TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    UNIQUE KEY uq_token (token(255))
) ENGINE=InnoDB;

CREATE TABLE practice_sessions (
    session_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    mode_id INT NOT NULL,
    is_random BOOLEAN DEFAULT TRUE,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (mode_id) REFERENCES practice_modes(mode_id)
) ENGINE=InnoDB;

/* ===== EXAM STRUCTURE ===== */

CREATE TABLE exam_structures (
    exam_structure_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    total_parts INT NOT NULL,
    description TEXT
) ENGINE=InnoDB;

CREATE TABLE parts (
    part_id INT AUTO_INCREMENT PRIMARY KEY,
    exam_structure_id INT NOT NULL,
    part_type_id INT NOT NULL,
    description TEXT,
    time_limit INT,
    min_words INT,
    max_words INT,
    FOREIGN KEY (exam_structure_id) REFERENCES exam_structures(exam_structure_id) ON DELETE CASCADE,
    FOREIGN KEY (part_type_id) REFERENCES part_types(part_type_id)
) ENGINE=InnoDB;

/* ===== TOPIC & CONTENT ===== */

CREATE TABLE topics (
    topic_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL,
    topic_name VARCHAR(255) NOT NULL,
    context TEXT,
    purpose TEXT,
    recipient_role VARCHAR(100),
    difficulty_level_id INT NOT NULL,
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE,
    FOREIGN KEY (difficulty_level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB;

CREATE TABLE vocabulary_sets (
    vocab_set_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL,
    level_id INT NOT NULL,
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB;

CREATE TABLE vocabulary_items (
    vocab_id INT AUTO_INCREMENT PRIMARY KEY,
    vocab_set_id INT NOT NULL,
    word VARCHAR(100) NOT NULL,
    meaning VARCHAR(255),
    example_sentence TEXT,
    FOREIGN KEY (vocab_set_id) REFERENCES vocabulary_sets(vocab_set_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE sentence_structures (
    structure_id INT AUTO_INCREMENT PRIMARY KEY,
    vocab_set_id INT NOT NULL,
    pattern VARCHAR(255) NOT NULL,
    usage_note TEXT,
    example TEXT,
    FOREIGN KEY (vocab_set_id) REFERENCES vocabulary_sets(vocab_set_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE sample_texts (
    sample_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL,
    level_id INT NOT NULL,
    sample_type_id INT NOT NULL,
    content TEXT NOT NULL,
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (sample_type_id) REFERENCES sample_types(sample_type_id)
) ENGINE=InnoDB;

/* ===== SUBMISSION & SUPPORT ===== */

CREATE TABLE user_submissions (
    submission_id INT AUTO_INCREMENT PRIMARY KEY,
    session_id INT NOT NULL,
    topic_id INT NOT NULL,
    part_id INT NOT NULL,
    content TEXT NOT NULL,
    word_count INT,
    enable_hint BOOLEAN DEFAULT FALSE,
    submitted_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (session_id) REFERENCES practice_sessions(session_id) ON DELETE CASCADE,
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id),
    FOREIGN KEY (part_id) REFERENCES parts(part_id)
) ENGINE=InnoDB;

CREATE TABLE hints (
    hint_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL,
    level_id INT NOT NULL,
    hint_type_id INT NOT NULL,
    content TEXT NOT NULL,
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (hint_type_id) REFERENCES hint_types(hint_type_id)
) ENGINE=InnoDB;

CREATE TABLE language_checks (
    check_id INT AUTO_INCREMENT PRIMARY KEY,
    submission_id INT UNIQUE NOT NULL,
    spelling_errors INT DEFAULT 0,
    grammar_errors INT DEFAULT 0,
    syntax_errors INT DEFAULT 0,
    feedback TEXT,
    FOREIGN KEY (submission_id) REFERENCES user_submissions(submission_id) ON DELETE CASCADE
) ENGINE=InnoDB;

/* ===== AI SCORING ===== */

CREATE TABLE scoring_criteria (
    criteria_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL,
    name VARCHAR(100) NOT NULL,
    weight FLOAT NOT NULL,
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ai_evaluations (
    evaluation_id INT AUTO_INCREMENT PRIMARY KEY,
    submission_id INT UNIQUE NOT NULL,
    total_score FLOAT,
    estimated_level_id INT,
    overall_feedback TEXT,
    FOREIGN KEY (submission_id) REFERENCES user_submissions(submission_id) ON DELETE CASCADE,
    FOREIGN KEY (estimated_level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB;

CREATE TABLE criteria_scores (
    criteria_score_id INT AUTO_INCREMENT PRIMARY KEY,
    evaluation_id INT NOT NULL,
    criteria_id INT NOT NULL,
    score FLOAT,
    feedback TEXT,
    FOREIGN KEY (evaluation_id) REFERENCES ai_evaluations(evaluation_id) ON DELETE CASCADE,
    FOREIGN KEY (criteria_id) REFERENCES scoring_criteria(criteria_id) ON DELETE CASCADE
) ENGINE=InnoDB;

/* ===== SYSTEM PROMPT ===== */

CREATE TABLE system_prompts (
    prompt_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL,
    level_id INT NOT NULL,
    purpose_id INT NOT NULL,
    prompt_content TEXT NOT NULL,
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (purpose_id) REFERENCES prompt_purposes(purpose_id)
) ENGINE=InnoDB;
