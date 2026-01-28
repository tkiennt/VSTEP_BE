/* =====================================
   VSTEP WRITING SYSTEM - Complete Database Setup
   Database: vstep_writing
   MySQL - utf8mb4
   Version: 1.0
   Last Updated: January 28, 2026
   ===================================== */

-- Drop database if exists (CAUTION: This will delete all data)
DROP DATABASE IF EXISTS vstep_writing;

-- Create database with UTF-8 support
CREATE DATABASE vstep_writing
CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE vstep_writing;

/* =====================================
   LOOKUP TABLES
   ===================================== */

-- Proficiency levels (A1, A2, B1, B2, C1, C2)
CREATE TABLE levels (
    level_id INT AUTO_INCREMENT PRIMARY KEY,
    level_code VARCHAR(5) UNIQUE NOT NULL COMMENT 'e.g., A1, A2, B1, B2, C1, C2',
    description VARCHAR(100) COMMENT 'Level description'
) ENGINE=InnoDB COMMENT='Proficiency levels for VSTEP exam';

-- Part types for exam structure
CREATE TABLE part_types (
    part_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(20) UNIQUE NOT NULL COMMENT 'Unique code for part type',
    description VARCHAR(100) COMMENT 'Part type description'
) ENGINE=InnoDB COMMENT='Types of exam parts';

-- Practice modes
CREATE TABLE practice_modes (
    mode_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL COMMENT 'Unique code for practice mode',
    description VARCHAR(100) COMMENT 'Practice mode description'
) ENGINE=InnoDB COMMENT='Different practice modes available';

-- Hint types for writing assistance
CREATE TABLE hint_types (
    hint_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL COMMENT 'Unique code for hint type',
    description VARCHAR(100) COMMENT 'Hint type description'
) ENGINE=InnoDB COMMENT='Types of hints available for students';

-- AI prompt purposes
CREATE TABLE prompt_purposes (
    purpose_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(30) UNIQUE NOT NULL COMMENT 'Unique code for prompt purpose',
    description VARCHAR(100) COMMENT 'Purpose description'
) ENGINE=InnoDB COMMENT='Purposes for AI system prompts';

-- Sample text types
CREATE TABLE sample_types (
    sample_type_id INT AUTO_INCREMENT PRIMARY KEY,
    code VARCHAR(20) UNIQUE NOT NULL COMMENT 'Unique code for sample type',
    description VARCHAR(100) COMMENT 'Sample type description'
) ENGINE=InnoDB COMMENT='Types of sample texts for reference';

/* =====================================
   USER MANAGEMENT & AUTHENTICATION
   ===================================== */

-- Users table with authentication support
CREATE TABLE users (
    user_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL COMMENT 'Full name of user',
    username VARCHAR(100) UNIQUE NOT NULL COMMENT 'Username for login (unique)',
    email VARCHAR(150) UNIQUE NOT NULL COMMENT 'Email address (unique)',
    phone_number VARCHAR(20) UNIQUE NULL COMMENT 'Phone number (optional)',
    password_hash VARCHAR(500) NOT NULL COMMENT 'BCrypt hashed password',
    role VARCHAR(20) NOT NULL DEFAULT 'Guest' COMMENT 'User role: Guest, User, Manager, Admin',
    target_level_id INT NOT NULL DEFAULT 1 COMMENT 'Target proficiency level',
    is_active TINYINT(1) NOT NULL DEFAULT 1 COMMENT 'Account active status',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Account creation timestamp',
    updated_at DATETIME NULL COMMENT 'Last update timestamp',
    FOREIGN KEY (target_level_id) REFERENCES levels(level_id),
    INDEX idx_username (username),
    INDEX idx_email (email),
    INDEX idx_role (role)
) ENGINE=InnoDB COMMENT='User accounts with authentication data';

-- Password reset tokens
CREATE TABLE password_reset_tokens (
    token_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL COMMENT 'User requesting password reset',
    token VARCHAR(500) NOT NULL COMMENT 'Secure reset token',
    expires_at DATETIME NOT NULL COMMENT 'Token expiration time (24 hours)',
    used TINYINT(1) NOT NULL DEFAULT 0 COMMENT 'Whether token has been used',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Token creation timestamp',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    UNIQUE KEY uq_token (token(255)),
    INDEX idx_token (token(255)),
    INDEX idx_user_expires (user_id, expires_at)
) ENGINE=InnoDB COMMENT='Password reset tokens with expiration';

-- Practice sessions
CREATE TABLE practice_sessions (
    session_id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL COMMENT 'User who created the session',
    mode_id INT NOT NULL COMMENT 'Practice mode used',
    is_random BOOLEAN DEFAULT TRUE COMMENT 'Whether topics are randomized',
    started_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Session start time',
    ended_at DATETIME NULL COMMENT 'Session end time',
    status VARCHAR(20) DEFAULT 'active' COMMENT 'Session status: active, completed, abandoned',
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (mode_id) REFERENCES practice_modes(mode_id),
    INDEX idx_user_status (user_id, status)
) ENGINE=InnoDB COMMENT='User practice sessions';

/* =====================================
   EXAM STRUCTURE
   ===================================== */

-- Exam structures
CREATE TABLE exam_structures (
    exam_structure_id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL COMMENT 'Exam structure name',
    description TEXT COMMENT 'Detailed description',
    total_parts INT NOT NULL COMMENT 'Total number of parts',
    duration_minutes INT COMMENT 'Total duration in minutes',
    is_active TINYINT(1) DEFAULT 1 COMMENT 'Whether this structure is currently active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB COMMENT='Exam structure definitions';

-- Parts of exams
CREATE TABLE parts (
    part_id INT AUTO_INCREMENT PRIMARY KEY,
    exam_structure_id INT NOT NULL COMMENT 'Parent exam structure',
    part_type_id INT NOT NULL COMMENT 'Type of this part',
    part_number INT NOT NULL COMMENT 'Part number in sequence',
    title VARCHAR(200) COMMENT 'Part title',
    description TEXT COMMENT 'Part description and instructions',
    instructions TEXT COMMENT 'Detailed instructions for students',
    time_limit INT COMMENT 'Time limit in minutes',
    min_words INT COMMENT 'Minimum word count',
    max_words INT COMMENT 'Maximum word count',
    FOREIGN KEY (exam_structure_id) REFERENCES exam_structures(exam_structure_id) ON DELETE CASCADE,
    FOREIGN KEY (part_type_id) REFERENCES part_types(part_type_id),
    INDEX idx_exam_part (exam_structure_id, part_number)
) ENGINE=InnoDB COMMENT='Individual parts of exam structures';

/* =====================================
   TOPICS & CONTENT
   ===================================== */

-- Writing topics
CREATE TABLE topics (
    topic_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL COMMENT 'Which part this topic belongs to',
    difficulty_level_id INT NOT NULL COMMENT 'Difficulty level',
    topic_name VARCHAR(255) NOT NULL COMMENT 'Topic title',
    description TEXT COMMENT 'Topic description',
    content TEXT COMMENT 'Full topic content/prompt',
    context TEXT COMMENT 'Context or background information',
    purpose TEXT COMMENT 'Purpose of the writing task',
    recipient_role VARCHAR(100) COMMENT 'Target recipient role (e.g., manager, friend)',
    min_words INT COMMENT 'Minimum word count',
    max_words INT COMMENT 'Maximum word count',
    is_active TINYINT(1) DEFAULT 1 COMMENT 'Whether topic is active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE,
    FOREIGN KEY (difficulty_level_id) REFERENCES levels(level_id),
    INDEX idx_part_difficulty (part_id, difficulty_level_id)
) ENGINE=InnoDB COMMENT='Writing topics for practice and exams';

-- Vocabulary sets for topics
CREATE TABLE vocabulary_sets (
    vocab_set_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL COMMENT 'Associated topic',
    level_id INT NOT NULL COMMENT 'Proficiency level',
    name VARCHAR(100) COMMENT 'Vocabulary set name',
    description TEXT COMMENT 'Set description',
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB COMMENT='Vocabulary sets for different topics and levels';

-- Individual vocabulary items
CREATE TABLE vocabulary_items (
    vocab_id INT AUTO_INCREMENT PRIMARY KEY,
    vocab_set_id INT NOT NULL COMMENT 'Parent vocabulary set',
    word VARCHAR(100) NOT NULL COMMENT 'The vocabulary word',
    meaning VARCHAR(255) COMMENT 'Word meaning/definition',
    example_sentence TEXT COMMENT 'Example sentence using the word',
    part_of_speech VARCHAR(20) COMMENT 'Part of speech (noun, verb, etc.)',
    FOREIGN KEY (vocab_set_id) REFERENCES vocabulary_sets(vocab_set_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Individual vocabulary items';

-- Sentence structures for writing guidance
CREATE TABLE sentence_structures (
    structure_id INT AUTO_INCREMENT PRIMARY KEY,
    vocab_set_id INT NOT NULL COMMENT 'Associated vocabulary set',
    pattern VARCHAR(255) NOT NULL COMMENT 'Sentence pattern/structure',
    usage_note TEXT COMMENT 'Usage notes and guidelines',
    example TEXT COMMENT 'Example sentence',
    FOREIGN KEY (vocab_set_id) REFERENCES vocabulary_sets(vocab_set_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Sentence structures for writing guidance';

-- Sample texts for reference
CREATE TABLE sample_texts (
    sample_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL COMMENT 'Related topic',
    level_id INT NOT NULL COMMENT 'Proficiency level',
    sample_type_id INT NOT NULL COMMENT 'Type of sample',
    content TEXT NOT NULL COMMENT 'Sample text content',
    title VARCHAR(200) COMMENT 'Sample title',
    author VARCHAR(100) COMMENT 'Author or source',
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (sample_type_id) REFERENCES sample_types(sample_type_id)
) ENGINE=InnoDB COMMENT='Sample texts for student reference';

/* =====================================
   SUBMISSIONS & SUPPORT
   ===================================== */

-- User submissions
CREATE TABLE user_submissions (
    submission_id INT AUTO_INCREMENT PRIMARY KEY,
    session_id INT NOT NULL COMMENT 'Practice session',
    topic_id INT NOT NULL COMMENT 'Topic being written about',
    part_id INT NOT NULL COMMENT 'Exam part',
    content TEXT NOT NULL COMMENT 'User writing content',
    word_count INT COMMENT 'Word count',
    enable_hint BOOLEAN DEFAULT FALSE COMMENT 'Whether hints were enabled',
    submitted_at DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Submission timestamp',
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (session_id) REFERENCES practice_sessions(session_id) ON DELETE CASCADE,
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id),
    FOREIGN KEY (part_id) REFERENCES parts(part_id),
    INDEX idx_session_topic (session_id, topic_id)
) ENGINE=InnoDB COMMENT='User writing submissions';

-- Hints for writing assistance
CREATE TABLE hints (
    hint_id INT AUTO_INCREMENT PRIMARY KEY,
    topic_id INT NOT NULL COMMENT 'Related topic',
    level_id INT NOT NULL COMMENT 'Proficiency level',
    hint_type_id INT NOT NULL COMMENT 'Type of hint',
    content TEXT NOT NULL COMMENT 'Hint content',
    display_order INT COMMENT 'Display order',
    FOREIGN KEY (topic_id) REFERENCES topics(topic_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (hint_type_id) REFERENCES hint_types(hint_type_id)
) ENGINE=InnoDB COMMENT='Hints to assist students with writing';

-- Language checks (grammar, spelling, etc.)
CREATE TABLE language_checks (
    check_id INT AUTO_INCREMENT PRIMARY KEY,
    submission_id INT UNIQUE NOT NULL COMMENT 'Submission being checked',
    spelling_errors INT DEFAULT 0 COMMENT 'Number of spelling errors',
    grammar_errors INT DEFAULT 0 COMMENT 'Number of grammar errors',
    syntax_errors INT DEFAULT 0 COMMENT 'Number of syntax errors',
    feedback TEXT COMMENT 'Detailed feedback',
    checked_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (submission_id) REFERENCES user_submissions(submission_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Language check results for submissions';

/* =====================================
   AI SCORING & EVALUATION
   ===================================== */

-- Scoring criteria for evaluation
CREATE TABLE scoring_criteria (
    criteria_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL COMMENT 'Related exam part',
    name VARCHAR(100) NOT NULL COMMENT 'Criteria name',
    description TEXT COMMENT 'Criteria description',
    weight FLOAT NOT NULL COMMENT 'Weight in total score (0-1)',
    max_score INT DEFAULT 10 COMMENT 'Maximum score for this criterion',
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Scoring criteria for AI evaluation';

-- AI evaluation results
CREATE TABLE ai_evaluations (
    evaluation_id INT AUTO_INCREMENT PRIMARY KEY,
    submission_id INT UNIQUE NOT NULL COMMENT 'Submission being evaluated',
    total_score FLOAT COMMENT 'Total score calculated',
    estimated_level_id INT COMMENT 'Estimated proficiency level',
    overall_feedback TEXT COMMENT 'Overall feedback from AI',
    strengths TEXT COMMENT 'Identified strengths',
    weaknesses TEXT COMMENT 'Identified weaknesses',
    suggestions TEXT COMMENT 'Suggestions for improvement',
    evaluated_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (submission_id) REFERENCES user_submissions(submission_id) ON DELETE CASCADE,
    FOREIGN KEY (estimated_level_id) REFERENCES levels(level_id)
) ENGINE=InnoDB COMMENT='AI evaluation results for submissions';

-- Individual criteria scores
CREATE TABLE criteria_scores (
    criteria_score_id INT AUTO_INCREMENT PRIMARY KEY,
    evaluation_id INT NOT NULL COMMENT 'Parent evaluation',
    criteria_id INT NOT NULL COMMENT 'Scoring criterion',
    score FLOAT COMMENT 'Score for this criterion',
    feedback TEXT COMMENT 'Feedback for this criterion',
    FOREIGN KEY (evaluation_id) REFERENCES ai_evaluations(evaluation_id) ON DELETE CASCADE,
    FOREIGN KEY (criteria_id) REFERENCES scoring_criteria(criteria_id) ON DELETE CASCADE
) ENGINE=InnoDB COMMENT='Individual scores for each criterion';

/* =====================================
   AI SYSTEM PROMPTS
   ===================================== */

-- System prompts for AI
CREATE TABLE system_prompts (
    prompt_id INT AUTO_INCREMENT PRIMARY KEY,
    part_id INT NOT NULL COMMENT 'Related exam part',
    level_id INT NOT NULL COMMENT 'Proficiency level',
    purpose_id INT NOT NULL COMMENT 'Prompt purpose',
    prompt_content TEXT NOT NULL COMMENT 'Prompt content for AI',
    is_active TINYINT(1) DEFAULT 1 COMMENT 'Whether prompt is active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (part_id) REFERENCES parts(part_id) ON DELETE CASCADE,
    FOREIGN KEY (level_id) REFERENCES levels(level_id),
    FOREIGN KEY (purpose_id) REFERENCES prompt_purposes(purpose_id)
) ENGINE=InnoDB COMMENT='System prompts for AI evaluation and assistance';

/* =====================================
   SEED DATA
   ===================================== */

-- Insert default proficiency levels
INSERT INTO levels (level_code, description) VALUES
('A1', 'Beginner - Basic phrases and simple expressions'),
('A2', 'Elementary - Simple everyday situations'),
('B1', 'Intermediate - Main points on familiar matters'),
('B2', 'Upper Intermediate - Complex texts and abstract topics'),
('C1', 'Advanced - Wide range of demanding texts'),
('C2', 'Proficient - Virtually everything with ease');

-- Insert part types
INSERT INTO part_types (code, description) VALUES
('writing_part1', 'Writing Part 1 - Email/Letter (150 words)'),
('writing_part2', 'Writing Part 2 - Essay (250 words)');

-- Insert practice modes
INSERT INTO practice_modes (code, description) VALUES
('full_test', 'Full Test - Complete exam simulation with all parts'),
('by_part', 'By Part - Practice individual parts separately'),
('timed', 'Timed Practice - Practice with time constraints'),
('untimed', 'Untimed Practice - Practice without time limits'),
('random', 'Random Topics - Randomly selected topics');

-- Insert hint types
INSERT INTO hint_types (code, description) VALUES
('vocabulary', 'Vocabulary suggestions'),
('grammar', 'Grammar tips'),
('structure', 'Writing structure guidance'),
('example', 'Example sentences'),
('topic_related', 'Topic-related information');

-- Insert prompt purposes
INSERT INTO prompt_purposes (code, description) VALUES
('evaluation', 'Evaluation and scoring'),
('feedback', 'Providing detailed feedback'),
('suggestion', 'Generating improvement suggestions'),
('hint_generation', 'Generating hints for students');

-- Insert sample types
INSERT INTO sample_types (code, description) VALUES
('model_answer', 'Model answer for reference'),
('good_example', 'Good example with explanation'),
('common_mistake', 'Common mistake to avoid'),
('band_sample', 'Sample for specific band score');

-- Insert a default admin user (password: Admin123!)
-- Note: Replace this with a secure password hash in production
INSERT INTO users (name, username, email, password_hash, role, target_level_id, is_active) VALUES
('System Administrator', 'admin', 'admin@vstep-writing.com', '$2a$11$8K1p/a0dL3LyU0mYYlRxU.ZKXvpQqfm5J7rXqNKX9.V.KfL8rNNdm', 'Admin', 3, 1);

/* =====================================
   INDEXES FOR PERFORMANCE
   ===================================== */

-- Additional indexes for better query performance
CREATE INDEX idx_sessions_user_date ON practice_sessions(user_id, started_at DESC);
CREATE INDEX idx_submissions_session ON user_submissions(session_id, submitted_at DESC);
CREATE INDEX idx_topics_active ON topics(is_active, part_id);
CREATE INDEX idx_password_reset_expires ON password_reset_tokens(expires_at, used);

/* =====================================
   DATABASE SETUP COMPLETE
   ===================================== */

-- Display success message
SELECT 'Database vstep_writing created successfully!' AS status;
SELECT COUNT(*) AS total_tables FROM information_schema.tables WHERE table_schema = 'vstep_writing';
